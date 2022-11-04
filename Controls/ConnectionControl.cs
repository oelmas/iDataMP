﻿using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Comms;

namespace MissionPlanner.Controls
{
    
    public partial class ConnectionControl : UserControl
    {
        public ConnectionControl()
        {
            InitializeComponent();
            this.CMB_baudrate.Items.AddRange( new object[]
            {
                "115200",
                "57600",
                "38400",
                "19200",
                "9600",
                "4800",
                "2400",
                "1200",
                "300",
                "110"
            });
            linkLabel1.Click += (sender, e) => { ShowLinkStats?.Invoke(this, EventArgs.Empty); };
        }

        //public ComboBox CMB_baudrate => cmb_Baud;
        public ID_ComboBox CMB_baudrate => cmb_Baud;

        public ID_ComboBox CMB_serialport => cmb_Connection;

        public ID_ComboBox CMB_sysid => cmb_sysid;

        public event EventHandler ShowLinkStats;

        /// <summary>
        ///     Called from the main form - set whether we are connected or not currently.
        ///     UI will be updated accordingly
        /// </summary>
        /// <param name="isConnected">Whether we are connected</param>
        public void IsConnected(bool isConnected)
        {
            linkLabel1.Visible = isConnected;
            cmb_Baud.Enabled = !isConnected;
            cmb_Connection.Enabled = !isConnected;

            UpdateSysIDS();
        }

        private void ConnectionControl_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void cmb_Connection_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var combo = sender as ComboBox;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight),
                    e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                    e.Bounds);

            var text = combo.Items[e.Index].ToString();
            if (!MainV2.MONO) text = text + " " + SerialPort.GetNiceName(text);

            e.Graphics.DrawString(text, e.Font,
                new SolidBrush(combo.ForeColor),
                new Point(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        public void UpdateSysIDS()
        {
            cmb_sysid.SelectedIndexChanged -= CMB_sysid_SelectedIndexChanged;

            var oldidx = cmb_sysid.SelectedIndex;

            cmb_sysid.Items.Clear();

            var selectidx = -1;

            foreach (var port in MainV2.Comports.ToArray())
            {
                var list = port.MAVlist.GetRawIDS();

                foreach (var item in list)
                {
                    var temp = new port_sysid { compid = item % 256, sysid = item / 256, port = port };

                    // exclude GCS's from the list
                    if (temp.compid == (int)MAVLink.MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                        continue;

                    var idx = cmb_sysid.Items.Add(temp);

                    if (temp.port == MainV2.comPort && temp.sysid == MainV2.comPort.sysidcurrent &&
                        temp.compid == MainV2.comPort.compidcurrent) selectidx = idx;
                }
            }

            if ( /*oldidx == -1 && */ selectidx != -1) cmb_sysid.SelectedIndex = selectidx;

            cmb_sysid.SelectedIndexChanged += CMB_sysid_SelectedIndexChanged;
        }

        private void CMB_sysid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_sysid.SelectedItem == null)
                return;

            var temp = (port_sysid)cmb_sysid.SelectedItem;

            foreach (var port in MainV2.Comports)
                if (port == temp.port)
                {
                    MainV2.comPort = port;
                    MainV2.comPort.sysidcurrent = temp.sysid;
                    MainV2.comPort.compidcurrent = temp.compid;

                    if (MainV2.comPort.MAV.param.TotalReceived < MainV2.comPort.MAV.param.TotalReported &&
                        /*MainV2.comPort.MAV.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_AUTOPILOT1 && */
                        !(ModifierKeys == Keys.Control))
                        MainV2.comPort.getParamList();

                    MainV2.View.Reload();
                }
        }

        private void cmb_sysid_Format(object sender, ListControlConvertEventArgs e)
        {
            var temp = (port_sysid)e.Value;
            var compid = (MAVLink.MAV_COMPONENT)temp.compid;
            var mavComponentHeader = "MAV_COMP_ID_";
            string mavComponentString = null;

            foreach (var port in MainV2.Comports)
                if (port == temp.port)
                {
                    if (compid == (MAVLink.MAV_COMPONENT)1)
                    {
                        //use Autopilot type as displaystring instead of "FCS1"
                        mavComponentString = port.MAVlist[temp.sysid, temp.compid].aptype.ToString();
                    }
                    else
                    {
                        //use name from enum if it exists, use the component ID otherwise
                        mavComponentString = compid.ToString();
                        if (mavComponentString.Length > mavComponentHeader.Length)
                            //remove "MAV_COMP_ID_" header
                            mavComponentString = mavComponentString.Remove(0, mavComponentHeader.Length);

                        if (temp.port.MAVlist[temp.sysid, temp.compid].CANNode)
                            mavComponentString =
                                temp.compid + " " + temp.port.MAVlist[temp.sysid, temp.compid].VersionString;
                    }

                    e.Value = temp.port.BaseStream.PortName + "-" + temp.sysid + "-" +
                              mavComponentString.Replace("_", " ");
                }
        }

        internal struct port_sysid
        {
            internal MAVLinkInterface port;
            internal int sysid;
            internal int compid;
        }
    }
}