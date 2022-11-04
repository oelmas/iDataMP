using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class MavlinkCheckBoxBitMask : MyUserControl
    {
        private int chkcount;

        private readonly List<KeyValuePair<int, CheckBox>> chklist = new List<KeyValuePair<int, CheckBox>>();
        public Label label1;
        private List<KeyValuePair<int, string>> list;
        public MyLabel myLabel1;
        public Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;

        private MAVLink.MAV_PARAM_TYPE Type = MAVLink.MAV_PARAM_TYPE.REAL32;

        public MavlinkCheckBoxBitMask()
        {
            InitializeComponent();

            Enabled = false;
            Width = 700;
        }

        [Browsable(true)] public string ParamName { get; set; }


        public float Value
        {
            get
            {
                float answer = 0;

                for (var a = 0; a < chklist.Count; a++)
                    answer += chklist[a].Value.Checked ? (uint)(1 << chklist[a].Key) : 0;

                // type conversions
                // ie int8 255 = -1
                if (Type == MAVLink.MAV_PARAM_TYPE.INT8)
                    answer = (sbyte)answer;
                else if (Type == MAVLink.MAV_PARAM_TYPE.INT16)
                    answer = (short)answer;
                else if (Type == MAVLink.MAV_PARAM_TYPE.INT32) answer = (int)answer;

                return answer;
            }
            set
            {
                for (var a = 0; a < chkcount; a++)
                {
                    var chk = (CheckBox)panel1.Controls[a];


                    chk.Checked = ((uint)value & (1 << list[a].Key)) > 0;
                }
            }
        }

        public event EventValueChanged ValueChanged;

        public void setup(string paramname, MAVLink.MAVLinkParamList paramlist)
        {
            ParamName = paramname;

            if (paramlist.ContainsKey(paramname))
            {
                Enabled = true;

                Name = paramname;

                myLabel1.Text = ParameterMetaDataRepository.GetParameterMetaData(paramname,
                    ParameterMetaDataConstants.DisplayName, MainV2.comPort.MAV.cs.firmware.ToString());
                label1.Text = ParameterMetaDataRepository.GetParameterMetaData(paramname,
                    ParameterMetaDataConstants.Description, MainV2.comPort.MAV.cs.firmware.ToString());

                list = ParameterMetaDataRepository.GetParameterBitMaskInt(ParamName,
                    MainV2.comPort.MAV.cs.firmware.ToString());
                chkcount = list.Count;

                var leftside = 9;
                var top = 9;
                var bottom = 0;

                var value = (uint)paramlist[paramname].Value;

                Type = paramlist[paramname].TypeAP;

                for (var a = 0; a < chkcount; a++)
                {
                    var chk = new CheckBox();
                    chk.AutoSize = true;
                    chk.Text = list[a].Value;
                    chk.Location = new Point(leftside, top);

                    bottom = chk.Bottom;

                    chk.CheckedChanged -= MavlinkCheckBoxBitMask_CheckedChanged;

                    if ((value & (1 << list[a].Key)) > 0) chk.Checked = true;

                    chklist.Add(new KeyValuePair<int, CheckBox>(list[a].Key, chk));
                    panel1.Controls.Add(chk);

                    chk.CheckedChanged += MavlinkCheckBoxBitMask_CheckedChanged;

                    //this.Controls.Add(new Label() { Location = chk.Location, Text = "test" });

                    leftside += chk.Width + 5;
                    if (leftside > 500)
                    {
                        top += chk.Height + 5;
                        leftside = 9;
                    }
                }


                panel1.Height = bottom;

                Height = myLabel1.Height + tableLayoutPanel1.Height + 25;
            }
            else
            {
                Enabled = false;
            }
        }

        private void MavlinkCheckBoxBitMask_CheckedChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender, ParamName, Value.ToString());
                return;
            }

            try
            {
                var ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                    ParamName, Value);
                if (ans == false)
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
            }
        }

        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            panel1 = new Panel();
            myLabel1 = new MyLabel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom
                                 | AnchorStyles.Left
                                 | AnchorStyles.Right;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Location = new Point(3, 28);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(0, 5, 0, 10);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(337, 84);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 5);
            label1.Name = "label1";
            label1.Size = new Size(35, 13);
            label1.TabIndex = 0;
            label1.Text = "label1";
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 21);
            panel1.Name = "panel1";
            panel1.Size = new Size(331, 50);
            panel1.TabIndex = 1;
            // 
            // myLabel1
            // 
            myLabel1.Anchor =
                AnchorStyles.Top | AnchorStyles.Left
                                 | AnchorStyles.Right;
            myLabel1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold);
            myLabel1.Location = new Point(3, 3);
            myLabel1.Name = "myLabel1";
            myLabel1.resize = false;
            myLabel1.Size = new Size(337, 23);
            myLabel1.TabIndex = 3;
            myLabel1.Text = "myLabel1";
            // 
            // MavlinkCheckBoxBitMask
            // 
            Controls.Add(tableLayoutPanel1);
            Controls.Add(myLabel1);
            Name = "MavlinkCheckBoxBitMask";
            Size = new Size(343, 115);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}