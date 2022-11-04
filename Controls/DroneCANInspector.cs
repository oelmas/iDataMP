using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DroneCAN;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Controls
{
    public class DroneCANInspector : Form
    {
        private MyButton but_graphit;
        private MyButton but_subscribe;
        private readonly DroneCAN.DroneCAN can;

        private readonly Color[] color =
            { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange };

        private IContainer components;
        private GroupBox groupBox1;

        private int history = 50;

        private readonly PacketInspector<(CANFrame frame, object message)>
            pktinspect = new PacketInspector<(CANFrame, object)>();

        private string selectedmsgid;
        private Timer timer1;
        private MyTreeView treeView1;

        public DroneCANInspector(DroneCAN.DroneCAN can)
        {
            InitializeComponent();

            this.can = can;

            can.MessageReceived += Can_MessageReceived;

            pktinspect.NewSysidCompid += (sender, args) => { };

            timer1.Tick += (sender, args) => Update();

            timer1.Start();

            ThemeManager.ApplyThemeTo(this);
        }

        private void Can_MessageReceived(CANFrame frame, object msg, byte transferID)
        {
            pktinspect.Add(frame.SourceNode, 0, frame.MsgTypeID, (frame, msg), frame.SizeofEntireMsg);
        }

        public new void Update()
        {
            treeView1.BeginUpdate();

            var added = false;

            foreach (var dronecanMessage in pktinspect.GetPacketMessages())
            {
                TreeNode sysidnode;
                TreeNode msgidnode;

                var sysidnodes = treeView1.Nodes.Find(dronecanMessage.frame.SourceNode.ToString(), false);
                if (sysidnodes.Length == 0)
                {
                    sysidnode = new TreeNode("ID " + dronecanMessage.frame.SourceNode)
                    {
                        Name = dronecanMessage.frame.SourceNode.ToString()
                    };
                    treeView1.Nodes.Add(sysidnode);
                    added = true;
                }
                else
                {
                    sysidnode = sysidnodes.First();
                    sysidnode.Text = "ID " + dronecanMessage.frame.SourceNode + " - " +
                                     can.GetNodeName(dronecanMessage.frame.SourceNode) + " " + pktinspect
                                         .SeenBps(dronecanMessage.frame.SourceNode, 0)
                                         .ToString("~0Bps");
                }

                var msgidnodes = sysidnode.Nodes.Find(dronecanMessage.frame.MsgTypeID.ToString(), false);
                if (msgidnodes.Length == 0)
                {
                    msgidnode = new TreeNode(dronecanMessage.frame.MsgTypeID.ToString())
                    {
                        Name = dronecanMessage.frame.MsgTypeID.ToString()
                    };
                    sysidnode.Nodes.Add(msgidnode);
                    added = true;
                }
                else
                {
                    msgidnode = msgidnodes.First();
                }

                var seenrate =
                    pktinspect.SeenRate(dronecanMessage.frame.SourceNode, 0, dronecanMessage.frame.MsgTypeID);

                var msgidheader = dronecanMessage.message.GetType().Name + " (" +
                                  seenrate.ToString("0.0 Hz") + ", #" + dronecanMessage.frame.MsgTypeID + ") " +
                                  pktinspect.SeenBps(dronecanMessage.frame.SourceNode, 0,
                                      dronecanMessage.frame.MsgTypeID).ToString("~0Bps");

                if (msgidnode.Text != msgidheader)
                    msgidnode.Text = msgidheader;

                var minfo = DroneCAN.DroneCAN.MSG_INFO.First(a => a.Item1 == dronecanMessage.Item2.GetType());
                var fields = minfo.Item1.GetFields().Where(f => !f.IsLiteral).ToArray();

                PopulateMSG(fields, msgidnode, dronecanMessage.message);
            }

            if (added)
                treeView1.Sort();

            treeView1.EndUpdate();
        }

        private static void PopulateMSG(FieldInfo[] Fields, TreeNode MsgIdNode, object message)
        {
            foreach (var field in Fields)
            {
                if (!MsgIdNode.Nodes.ContainsKey(field.Name)) MsgIdNode.Nodes.Add(new TreeNode { Name = field.Name });

                var value = field.GetValue(message);

                if (field.Name == "time_unix_usec")
                {
                    var date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    try
                    {
                        value = date1.AddMilliseconds((ulong)value / 1000);
                    }
                    catch
                    {
                    }
                }

                if (field.FieldType.IsArray)
                {
                    var subtype = value.GetType();

                    var value2 = (Array)value;

                    if (field.Name == "param_id" || field.Name == "text" ||
                        field.Name == "string_value" || field.Name == "name") // param_value
                    {
                        value = Encoding.ASCII.GetString((byte[])value2);
                    }
                    else if (value2.Length > 0)
                    {
                        if (field.FieldType.IsClass)
                        {
                            var elementtype = field.FieldType.GetElementType();
                            var fields = elementtype.GetFields().Where(f => !f.IsLiteral).ToArray();

                            if (!elementtype.IsPrimitive)
                            {
                                MsgIdNode.Nodes[field.Name].Text = field.Name;
                                var a = 0;
                                foreach (var valuei in value2)
                                {
                                    var name = field.Name + "[" + a + "]";
                                    if (!MsgIdNode.Nodes[field.Name].Nodes.ContainsKey(name))
                                        MsgIdNode.Nodes[field.Name].Nodes.Add(new TreeNode
                                        {
                                            Name = name,
                                            Text = name
                                        });

                                    PopulateMSG(fields, MsgIdNode.Nodes[field.Name].Nodes[name], valuei);
                                    a++;
                                }

                                continue;
                            }
                        }

                        value = value2.Cast<object>().Aggregate((a, b) => a + "," + b);
                    }
                    else if (value2.Length == 0)
                    {
                        value = null;
                    }
                }

                if (!field.FieldType.IsArray && field.FieldType.IsClass)
                {
                    MsgIdNode.Nodes[field.Name].Text = field.Name;
                    PopulateMSG(field.FieldType.GetFields(), MsgIdNode.Nodes[field.Name], value);
                    continue;
                }

                MsgIdNode.Nodes[field.Name].Text = string.Format("{0,-32} {1,20} {2,-20}", field.Name, value,
                    field.FieldType.Name);
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
            treeView1 = new MyTreeView();
            groupBox1 = new GroupBox();
            timer1 = new Timer(components);
            but_graphit = new MyButton();
            but_subscribe = new MyButton();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeView1.Location = new Point(3, 16);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(693, 259);
            treeView1.TabIndex = 0;
            treeView1.DrawNode += treeView1_DrawNode;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                                                | AnchorStyles.Left
                                                | AnchorStyles.Right;
            groupBox1.Controls.Add(treeView1);
            groupBox1.Location = new Point(0, 30);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(699, 278);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            // 
            // timer1
            // 
            timer1.Interval = 333;
            // 
            // but_graphit
            // 
            but_graphit.Enabled = false;
            but_graphit.Location = new Point(12, 3);
            but_graphit.Name = "but_graphit";
            but_graphit.Size = new Size(75, 23);
            but_graphit.TabIndex = 4;
            but_graphit.Text = "Graph It";
            but_graphit.UseVisualStyleBackColor = true;
            but_graphit.Click += but_graphit_Click;
            // 
            // but_subscribe
            // 
            but_subscribe.Location = new Point(93, 3);
            but_subscribe.Name = "but_subscribe";
            but_subscribe.Size = new Size(75, 23);
            but_subscribe.TabIndex = 5;
            but_subscribe.Text = "Subscribe";
            but_subscribe.UseVisualStyleBackColor = true;
            but_subscribe.Click += but_subscribe_Click;
            // 
            // UAVCANInspector
            // 
            ClientSize = new Size(698, 311);
            Controls.Add(but_subscribe);
            Controls.Add(but_graphit);
            Controls.Add(groupBox1);
            Name = "UAVCANInspector";
            Text = "UAVCAN Inspector";
            FormClosing += MAVLinkInspector_FormClosing;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Y < 0 || e.Bounds.X == -1)
                return;

            var tv = sender as TreeView;

            new SolidBrush(Color.FromArgb(e.Bounds.Y % 200, e.Bounds.Y % 200, e.Bounds.Y % 200));

            e.Graphics.DrawString(e.Node.Text, tv.Font, new SolidBrush(ForeColor)
                , e.Bounds.X,
                e.Bounds.Y);
        }

        private void MAVLinkInspector_FormClosing(object sender, FormClosingEventArgs e)
        {
            can.MessageReceived -= Can_MessageReceived;

            timer1.Stop();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e == null || e.Node == null || e.Node.Parent == null)
                return;

            var throwaway = 0;
            //if (int.TryParse(e.Node.Parent.Name, out throwaway))
            {
                selectedmsgid = e.Node.Name;
                var current = e.Node.Parent;
                while (current != null)
                {
                    selectedmsgid = current.Name + "/" + selectedmsgid;
                    current = current.Parent;
                }

                but_graphit.Enabled = true;
                but_subscribe.Enabled = true;
            }
            //else
            {
                // but_graphit.Enabled = false;
            }
        }

        private void but_graphit_Click(object sender, EventArgs e)
        {
            InputBox.Show("Points", "Points of history?", ref history);
            var form = new Form { Size = new Size(640, 480) };
            var zg1 = new ZedGraphControl { Dock = DockStyle.Fill };
            var msgpath = selectedmsgid.Split('/');
            var nodeid = int.Parse(msgpath[0]);
            var msgid = int.Parse(msgpath[1]);
            var path = msgpath.Skip(2);
            var msgidfield = msgpath.Last();
            var line = new LineItem(msgidfield, new RollingPointPairList(history), Color.Red, SymbolType.None);
            zg1.GraphPane.Title.Text = "";

            zg1.GraphPane.CurveList.Add(line);

            zg1.GraphPane.XAxis.Type = AxisType.Date;
            zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss.fff";
            zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;

            var timer = new Timer { Interval = 100 };
            DroneCAN.DroneCAN.MessageRecievedDel msgrecv = (frame, msg, id) =>
            {
                if (frame.SourceNode == nodeid && frame.MsgTypeID == msgid)
                {
                    var data = msg;
                    foreach (var subpath in path)
                    {
                        // array member
                        if (subpath.Contains("["))
                        {
                            var count = subpath.Split('[', ']');
                            var index = int.Parse(count[1]);
                            data = ((IList)data)[index];
                            continue;
                        }

                        var field = data.GetType().GetField(subpath);
                        data = field.GetValue(data);
                    }

                    var item = data;

                    if (item.GetType().IsClass && !item.GetType().IsArray)
                    {
                        var items = data.GetType().GetFields();
                        var dict = items.ToDictionary(ks => ks.Name, es => es.GetValue(item));

                        zg1.GraphPane.CurveList.Remove(line);

                        var a = 0;
                        foreach (var newitem in dict)
                        {
                            var label = msgidfield + "." + newitem.Key;
                            var lines = zg1.GraphPane.CurveList.Where(ci =>
                                ci.Label.Text == label || ci.Label.Text.StartsWith(label + " "));
                            if (lines.Count() == 0)
                            {
                                line = new LineItem(label, new RollingPointPairList(history), color[a % color.Length],
                                    SymbolType.None);
                                zg1.GraphPane.CurveList.Add(line);
                            }
                            else
                            {
                                line = (LineItem)lines.First();
                            }

                            AddToGraph(newitem.Value, zg1, newitem.Key, line);
                            a++;
                        }

                        return;
                    }

                    AddToGraph(item, zg1, msgidfield, line);
                }
            };
            can.MessageReceived += msgrecv;
            timer.Tick += (o, args) =>
            {
                // Make sure the Y axis is rescaled to accommodate actual data
                zg1.AxisChange();

                // Force a redraw

                zg1.Invalidate();
            };
            form.Controls.Add(zg1);
            form.Closing += (o2, args2) => { can.MessageReceived -= msgrecv; };
            ThemeManager.ApplyThemeTo(form);
            form.Show(this);
            timer.Start();
            but_graphit.Enabled = false;
        }

        private void AddToGraph(object item, ZedGraphControl zg1, string msgidfield, LineItem line)
        {
            if (item is IEnumerable)
            {
                var a = 0;
                foreach (var subitem in (IEnumerable)item)
                    if (subitem is IConvertible)
                    {
                        while (zg1.GraphPane.CurveList.Count < a + 1)
                            zg1.GraphPane.CurveList.Add(new LineItem(msgidfield + "[" + a + "]",
                                new RollingPointPairList(history), color[a % color.Length],
                                SymbolType.None));

                        zg1.GraphPane.CurveList[a].AddPoint(new XDate(DateTime.Now),
                            ((IConvertible)subitem).ToDouble(null));
                        a++;
                    }
            }
            else if (item is IConvertible)
            {
                line.AddPoint(new XDate(DateTime.Now),
                    ((IConvertible)item).ToDouble(null));
            }
            else if (item.GetType().IsClass)
            {
            }
            else
            {
                line.AddPoint(new XDate(DateTime.Now),
                    (double)(dynamic)item);
            }
        }

        private void but_subscribe_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedmsgid))
                return;
            new DroneCANSubscriber(can, selectedmsgid).ShowUserControl();
        }

        public class MyTreeView : TreeView
        {
            public MyTreeView()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                //UpdateStyles();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (GetStyle(ControlStyles.UserPaint))
                {
                    var m = new Message
                    {
                        HWnd = Handle
                    };
                    var WM_PRINTCLIENT = 0x318;
                    m.Msg = WM_PRINTCLIENT;
                    m.WParam = e.Graphics.GetHdc();
                    var PRF_CLIENT = 0x00000004;
                    m.LParam = (IntPtr)PRF_CLIENT;
                    DefWndProc(ref m);
                    e.Graphics.ReleaseHdc(m.WParam);
                }

                base.OnPaint(e);
            }
        }
    }
}