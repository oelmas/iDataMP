using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using ZedGraph;

namespace MissionPlanner.Controls
{
    public class MAVLinkInspector : Form
    {
        private MyButton but_graphit;
        private CheckBox chk_gcstraffic;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private IContainer components;
        private GroupBox groupBox1;

        private int history = 50;
        private readonly MAVLinkInterface mav;
        private readonly PacketInspector<MAVLink.MAVLinkMessage> mavi = new PacketInspector<MAVLink.MAVLinkMessage>();

        private string selectedmsgid;
        private Timer timer1;
        private MyTreeView treeView1;

        public MAVLinkInspector(MAVLinkInterface mav)
        {
            InitializeComponent();

            this.mav = mav;

            mav.OnPacketReceived += MavOnOnPacketReceived;

            mavi.NewSysidCompid += (sender, args) =>
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    comboBox1.DataSource = mavi.SeenSysid();
                    comboBox2.DataSource = mavi.SeenCompid();
                });
            };

            timer1.Tick += (sender, args) => Update();

            timer1.Start();

            ThemeManager.ApplyThemeTo(this);
        }

        private void MavOnOnPacketReceived(object o, MAVLink.MAVLinkMessage linkMessage)
        {
            mavi.Add(linkMessage.sysid, linkMessage.compid, linkMessage.msgid, linkMessage, linkMessage.Length);
        }

        public new void Update()
        {
            treeView1.BeginUpdate();

            var added = false;

            foreach (var mavLinkMessage in mavi.GetPacketMessages())
            {
                TreeNode sysidnode;
                TreeNode compidnode;
                TreeNode msgidnode;

                var sysidnodes = treeView1.Nodes.Find(mavLinkMessage.sysid.ToString(), false);
                if (sysidnodes.Length == 0)
                {
                    sysidnode = new TreeNode("Vehicle " + mavLinkMessage.sysid)
                    {
                        Name = mavLinkMessage.sysid.ToString()
                    };
                    treeView1.Nodes.Add(sysidnode);
                    added = true;
                }
                else
                {
                    sysidnode = sysidnodes.First();
                }

                var compidnodes = sysidnode.Nodes.Find(mavLinkMessage.compid.ToString(), false);
                if (compidnodes.Length == 0)
                {
                    compidnode = new TreeNode("Comp " + mavLinkMessage.compid + " " +
                                              (MAVLink.MAV_COMPONENT)mavLinkMessage.compid)
                    {
                        Name = mavLinkMessage.compid.ToString()
                    };
                    sysidnode.Nodes.Add(compidnode);
                    added = true;
                }
                else
                {
                    compidnode = compidnodes.First();
                }

                var msgidnodes = compidnode.Nodes.Find(mavLinkMessage.msgid.ToString(), false);
                if (msgidnodes.Length == 0)
                {
                    msgidnode = new TreeNode(mavLinkMessage.msgtypename)
                    {
                        Name = mavLinkMessage.msgid.ToString()
                    };
                    compidnode.Nodes.Add(msgidnode);
                    added = true;
                }
                else
                {
                    msgidnode = msgidnodes.First();
                }

                var msgidheader = mavLinkMessage.msgtypename + " (" +
                                  mavi.SeenRate(mavLinkMessage.sysid, mavLinkMessage.compid, mavLinkMessage.msgid)
                                      .ToString("0.0 Hz") + ", #" + mavLinkMessage.msgid + ") " +
                                  mavi.SeenBps(mavLinkMessage.sysid, mavLinkMessage.compid, mavLinkMessage.msgid)
                                      .ToString("0Bps");

                if (msgidnode.Text != msgidheader)
                    msgidnode.Text = msgidheader;

                var minfo = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo(mavLinkMessage.msgid);
                if (minfo.type == null)
                    continue;

                foreach (var field in minfo.type.GetFields())
                {
                    if (!msgidnode.Nodes.ContainsKey(field.Name))
                    {
                        msgidnode.Nodes.Add(new TreeNode { Name = field.Name });
                        added = true;
                    }

                    var value = field.GetValue(mavLinkMessage.data);

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

                        if (field.Name == "param_id" || field.Name == "text" || field.Name == "model_name" ||
                            field.Name == "vendor_name" || field.Name == "uri" ||
                            field.Name == "cam_definition_uri") // param_value
                            value = Encoding.ASCII.GetString((byte[])value2);
                        else
                            value = value2.Cast<object>().Aggregate((a, b) => a + "," + b);
                    }

                    msgidnode.Nodes[field.Name].Tag = new[]
                    {
                        field.Name, value,
                        field.FieldType.ToString()
                    };
                    msgidnode.Nodes[field.Name].Text = string.Format("{0,-32} {1,20} {2,-20}", field.Name, value,
                        field.FieldType);
                }
            }

            if (added)
                treeView1.Sort();

            treeView1.EndUpdate();
        }

        private void InitializeComponent()
        {
            components = new Container();
            treeView1 = new MyTreeView();
            groupBox1 = new GroupBox();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            timer1 = new Timer(components);
            but_graphit = new MyButton();
            chk_gcstraffic = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeView1.FullRowSelect = true;
            treeView1.Location = new Point(3, 16);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(521, 259);
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
            groupBox1.Size = new Size(527, 278);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(206, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 21);
            comboBox1.TabIndex = 2;
            comboBox1.Visible = false;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(356, 3);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(121, 21);
            comboBox2.TabIndex = 3;
            comboBox2.Visible = false;
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
            // chk_gcstraffic
            // 
            chk_gcstraffic.AutoSize = true;
            chk_gcstraffic.Location = new Point(93, 5);
            chk_gcstraffic.Name = "chk_gcstraffic";
            chk_gcstraffic.Size = new Size(111, 17);
            chk_gcstraffic.TabIndex = 5;
            chk_gcstraffic.Text = "Show GCS Traffic";
            chk_gcstraffic.UseVisualStyleBackColor = true;
            chk_gcstraffic.CheckedChanged += chk_gcstraffic_CheckedChanged;
            // 
            // MAVLinkInspector
            // 
            ClientSize = new Size(526, 311);
            Controls.Add(chk_gcstraffic);
            Controls.Add(but_graphit);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(groupBox1);
            Name = "MAVLinkInspector";
            Text = "Mavlink Inspector";
            FormClosing += MAVLinkInspector_FormClosing;
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Y < 0 || e.Bounds.X == -1)
                return;

            var tv = sender as TreeView;

            if (e.Node.Tag == null)
            {
                e.DrawDefault = true;
                return;
            }

            var items = (object[])e.Node.Tag;

            //(String.Format("{0,-32} {1,20} {2,-20}", field.Name, value, field.FieldType.ToString()));

            e.Graphics.DrawString(items[0].ToString(), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X,
                e.Bounds.Y);

            e.Graphics.DrawString(items[1].ToString().PadLeft(20, ' '), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X + tv.Width * 0.4f,
                e.Bounds.Y);

            e.Graphics.DrawString(items[2].ToString(), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X + tv.Width * 0.75f,
                e.Bounds.Y);
        }

        private void MAVLinkInspector_FormClosing(object sender, FormClosingEventArgs e)
        {
            mav.OnPacketReceived -= MavOnOnPacketReceived;
            mav.OnPacketSent -= MavOnOnPacketReceived;

            timer1.Stop();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e == null || e.Node == null || e.Node.Parent == null)
                return;

            var throwaway = 0;
            if (int.TryParse(e.Node.Parent.Name, out throwaway))
            {
                selectedmsgid = e.Node.FullPath;
                but_graphit.Enabled = true;
            }
            else
            {
                but_graphit.Enabled = false;
            }
        }

        private void but_graphit_Click(object sender, EventArgs e)
        {
            InputBox.Show("Points", "Points of history?", ref history);
            var form = new Form { Size = new Size(640, 480) };
            var zg1 = new ZedGraphControl { Dock = DockStyle.Fill };
            var path = selectedmsgid.Split('\\');
            if (path.Length < 4)
                return;

            var sysid = int.Parse(path[0].Split(' ')[1]);
            var compid = int.Parse(path[1].Split(' ')[1]);
            var msgt = path[2];
            var field = path[3];

            var msgid = int.Parse(msgt.Split('#', ')')[1]);
            var msgidfield = field.Split(' ')[0];

            var line = new LineItem(msgt.Split(' ')[0] + "." + msgidfield, new RollingPointPairList(history), Color.Red,
                SymbolType.None);
            zg1.GraphPane.Title.Text = "";
            try
            {
                var msginfo = MAVLink.MAVLINK_MESSAGE_INFOS.First(a => a.msgid == msgid);
                var typeofthing = msginfo.type.GetField(
                    msgidfield);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false);
                    if (attrib.Length > 0)
                        zg1.GraphPane.YAxis.Title.Text = attrib.OfType<MAVLink.Units>().First().Unit;
                }
            }
            catch
            {
            }

            zg1.GraphPane.CurveList.Add(line);

            zg1.GraphPane.XAxis.Type = AxisType.Date;
            zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss.fff";
            zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;

            Color[] color = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Violet, Color.Orange };
            var timer = new Timer { Interval = 100 };

            EventHandler<MAVLink.MAVLinkMessage> opr = null;
            opr = (e2, msg) =>
            {
                if (msg.msgid != msgid)
                    return;
                if (msg.sysid != sysid)
                    return;
                if (msg.compid != compid)
                    return;

                var item = msg.data.GetPropertyOrField(msgidfield);
                if (item is IEnumerable)
                {
                    var a = 0;
                    foreach (var subitem in (IEnumerable)item)
                        if (subitem is IConvertible)
                        {
                            while (zg1.GraphPane.CurveList.Count < a + 1)
                                zg1.GraphPane.CurveList.Add(new LineItem(msgidfield + "[" + a + "]",
                                    new RollingPointPairList(history), color[a % color.Length], SymbolType.None));

                            zg1.GraphPane.CurveList[a].AddPoint(new XDate(msg.rxtime),
                                ((IConvertible)subitem).ToDouble(null));
                            a++;
                        }
                }
                else if (item is IConvertible)
                {
                    line.AddPoint(new XDate(msg.rxtime),
                        ((IConvertible)item).ToDouble(null));
                }
                else
                {
                    line.AddPoint(new XDate(msg.rxtime),
                        (double)(dynamic)item);
                }
            };

            mav.OnPacketReceived += opr;
            mav.OnPacketSent += opr;

            timer.Tick += (o, args) =>
            {
                // Make sure the Y axis is rescaled to accommodate actual data
                zg1.AxisChange();

                // Force a redraw

                zg1.Invalidate();
            };
            form.Controls.Add(zg1);
            form.Closing += (o2, args2) =>
            {
                mav.OnPacketReceived -= opr;
                mav.OnPacketSent -= opr;
            };
            ThemeManager.ApplyThemeTo(form);
            form.Show(this);
            timer.Start();
            but_graphit.Enabled = false;
        }

        private void chk_gcstraffic_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_gcstraffic.Checked)
                mav.OnPacketSent += MavOnOnPacketReceived;
            if (!chk_gcstraffic.Checked)
            {
                mav.OnPacketSent -= MavOnOnPacketReceived;
                mav.OnPacketSent -= MavOnOnPacketReceived;
            }
        }

        public class MyTreeView : TreeView
        {
            public MyTreeView()
            {
                DoubleBuffered = true;
            }
        }
    }
}