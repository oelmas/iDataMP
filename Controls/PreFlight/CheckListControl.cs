using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls.PreFlight
{
    public partial class CheckListControl : UserControl
    {
        public List<CheckListItem> CheckListItems = new List<CheckListItem>();

        public string configfile = Settings.GetUserDataDirectory() + "checklist.xml";

        public string configfiledefault = Settings.GetRunningDirectory() + "checklistDefault.xml";

        private int rowcount;


        public CheckListControl()
        {
            InitializeComponent();

            try
            {
                CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;

                LoadConfig();
            }
            catch
            {
                Console.WriteLine("Failed to read CheckList config file " + configfile);
            }

            timer1.Start();
        }

        public void Draw()
        {
            lock (CheckListItems)
            {
                if (rowcount == CheckListItems.Count)
                    return;

                panel1.Visible = false;
                panel1.Controls.Clear();

                var y = 0;

                rowcount = 0;

                foreach (var item in CheckListItems)
                {
                    var wrnctl = addwarningcontrol(5, y, item);

                    rowcount++;

                    y = wrnctl.Bottom;
                }
            }

            panel1.Visible = true;
        }

        private void UpdateDisplay()
        {
            foreach (Control itemp in panel1.Controls)
            foreach (Control item in itemp.Controls)
            {
                if (item.Tag == null)
                    continue;

                var data = (internaldata)item.Tag;

                if (item.Name.StartsWith("utext"))
                {
                    item.Text = data.CLItem.DisplayText();
                    data.desc.Text = data.CLItem.Description;
                }

                if (item.Name.StartsWith("utickbox"))
                {
                    var tickbox = item as CheckBox;
                    if (data.CLItem.ConditionType != CheckListItem.Conditional.NONE)
                        tickbox.Checked = data.CLItem.checkCond(data.CLItem);

                    if (tickbox.Checked)
                    {
                        data.text.ForeColor = data.CLItem._TrueColor;
                        data.desc.ForeColor = data.CLItem._TrueColor;
                    }
                    else
                    {
                        data.text.ForeColor = data.CLItem._FalseColor;
                        data.desc.ForeColor = data.CLItem._FalseColor;
                    }
                }
            }
        }

        private Control addwarningcontrol(int x, int y, CheckListItem item, bool hideforchild = false)
        {
            var desctext = item.Description;
            var texttext = item.DisplayText();

            var height = TextRenderer.MeasureText(desctext, Font).Height;

            var x0 = (int)(Width * 0.95);
            var x1 = (int)(x0 * 0.5);
            var x2 = (int)(x0 * 0.4);
            var x3 = (int)(x0 * 0.1);

            var gb = new GroupBox
                { Text = "", Location = new Point(x, y), Size = new Size(x0, 17 + height), Name = "gb" + y };

            var desc = new Label
                { Text = desctext, Location = new Point(5, 9), Size = new Size(x1, height), Name = "udesc" + y };
            var text = new Label
            {
                Text = texttext, Location = new Point(desc.Right, 9), Size = new Size(x2, height), Name = "utext" + y
            };
            var tickbox = new CheckBox
            {
                Checked = item.checkCond(item), Location = new Point(text.Right, 7), Size = new Size(21, 21),
                Name = "utickbox" + y
            };

            desc.Tag = text.Tag = tickbox.Tag = new internaldata
                { CLItem = item, desc = desc, text = text, tickbox = tickbox };

            gb.Controls.Add(desc);
            gb.Controls.Add(text);
            gb.Controls.Add(tickbox);

            panel1.Controls.Add(gb);

            y = gb.Bottom;

            if (item.Child != null)
            {
                //return addwarningcontrol(x += 5, y, item.Child, true);
            }

            return gb;
        }

        public void LoadConfig()
        {
            var loadfile = configfile;

            if (!File.Exists(configfile))
            {
                if (!File.Exists(configfiledefault))
                    return;
                loadfile = configfiledefault;
            }

            var reader =
                new XmlSerializer(typeof(List<CheckListItem>),
                    new[] { typeof(CheckListItem) });

            using (var sr = new StreamReader(loadfile))
            {
                CheckListItems = (List<CheckListItem>)reader.Deserialize(sr);
            }
        }

        public void SaveConfig()
        {
            // save config
            var writer =
                new XmlSerializer(typeof(List<CheckListItem>),
                    new[] { typeof(CheckListItem), typeof(Color) });

            using (var sw = new StreamWriter(configfile))
            {
                lock (CheckListItems)
                {
                    writer.Serialize(sw, CheckListItems);
                }
            }
        }

        private void BUT_edit_Click(object sender, EventArgs e)
        {
            var form = new CheckListEditor(this);
            form.Show();
            lock (CheckListItems)
            {
                rowcount = 0;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Draw();
            UpdateDisplay();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!MainV2.DisplayConfiguration.displayPreFlightTabEdit) BUT_edit.Visible = false;

            if (Visible)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;
        }

        internal struct internaldata
        {
            internal Label desc;
            internal Label text;
            internal CheckBox tickbox;
            internal CheckListItem CLItem;
        }
    }
}