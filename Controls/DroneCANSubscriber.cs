using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DroneCAN;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace MissionPlanner.Controls
{
    public class DroneCANSubscriber : MyUserControl, IActivate, IDeactivate
    {
        private readonly DroneCAN.DroneCAN can;
        private ComboBox cmb_msg;

        private readonly List<string> msgtypes = new List<string>();
        private NumericUpDown num_lines;
        private string selectedmsgid;
        private string targettype;
        private TextBox txt_packet;

        public DroneCANSubscriber(DroneCAN.DroneCAN can, string selectedmsgid)
        {
            this.selectedmsgid = selectedmsgid;
            this.can = can;
            InitializeComponent();
        }

        public void Activate()
        {
            cmb_msg.Items.Clear();

            can.MessageReceived += Can_MessageReceived;

            var tim = new Timer { Interval = 1000, Enabled = true };

            tim.Tick += (sender, e) =>
            {
                foreach (var item in msgtypes)
                    if (!cmb_msg.Items.Contains(item))
                        cmb_msg.Items.Add(item);
            };
        }

        public void Deactivate()
        {
            can.MessageReceived -= Can_MessageReceived;
        }


        private void InitializeComponent()
        {
            cmb_msg = new ComboBox();
            num_lines = new NumericUpDown();
            txt_packet = new TextBox();
            ((ISupportInitialize)num_lines).BeginInit();
            SuspendLayout();
            // 
            // cmb_msg
            // 
            cmb_msg.Anchor = AnchorStyles.Top | AnchorStyles.Left
                                              | AnchorStyles.Right;
            cmb_msg.FormattingEnabled = true;
            cmb_msg.Location = new Point(124, 3);
            cmb_msg.Name = "cmb_msg";
            cmb_msg.Size = new Size(221, 21);
            cmb_msg.TabIndex = 0;
            cmb_msg.SelectedIndexChanged += cmb_msg_SelectedIndexChanged;
            // 
            // num_lines
            // 
            num_lines.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            num_lines.Location = new Point(351, 4);
            num_lines.Maximum = new decimal(new[]
            {
                10000,
                0,
                0,
                0
            });
            num_lines.Name = "num_lines";
            num_lines.Size = new Size(61, 20);
            num_lines.TabIndex = 1;
            num_lines.Value = new decimal(new[]
            {
                100,
                0,
                0,
                0
            });
            // 
            // txt_packet
            // 
            txt_packet.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                                                 | AnchorStyles.Left
                                                 | AnchorStyles.Right;
            txt_packet.Location = new Point(3, 30);
            txt_packet.Multiline = true;
            txt_packet.Name = "txt_packet";
            txt_packet.ScrollBars = ScrollBars.Both;
            txt_packet.Size = new Size(409, 299);
            txt_packet.TabIndex = 2;
            // 
            // UAVCANSubscriber
            // 
            Controls.Add(txt_packet);
            Controls.Add(num_lines);
            Controls.Add(cmb_msg);
            Name = "UAVCANSubscriber";
            Size = new Size(419, 337);
            ((ISupportInitialize)num_lines).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void cmb_msg_SelectedIndexChanged(object sender, EventArgs e)
        {
            targettype = cmb_msg.Text;
        }

        private void Can_MessageReceived(CANFrame frame, object msg, byte transferID)
        {
            if (msg.GetType().Name == targettype)
                this.BeginInvokeIfRequired(() =>
                {
                    var item = msg.ToJSON(Formatting.Indented);

                    var lines = item.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    var newlines = txt_packet.Lines.OfType<string>().ToList();
                    newlines.AddRange(lines);

                    if (newlines.Count > num_lines.Value)
                        txt_packet.Lines = newlines.Skip(txt_packet.Lines.Length - (int)num_lines.Value).ToArray();
                    else
                        txt_packet.Lines = newlines.ToArray();
                });

            if (!msgtypes.Contains(msg.GetType().Name)) msgtypes.Add(msg.GetType().Name);
        }
    }
}