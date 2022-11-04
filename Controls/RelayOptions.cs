using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class RelayOptions : UserControl
    {
        // start at 0 increment each instance
        private static int relay;

        public RelayOptions()
        {
            InitializeComponent();

            thisrelay = relay;

            TXT_rcchannel.Text = "Relay " + thisrelay;

            loadSettings();

            relay++;

            TXT_rcchannel.BackColor = Color.Gray;
        }

        public int thisrelay { get; set; }

        private void loadSettings()
        {
            var desc = Settings.Instance["Relay" + thisrelay + "_desc"];

            var highdesc = Settings.Instance["Relay" + thisrelay + "_highdesc"];
            var lowdesc = Settings.Instance["Relay" + thisrelay + "_lowdesc"];

            if (!string.IsNullOrEmpty(desc)) TXT_rcchannel.Text = desc;

            if (!string.IsNullOrEmpty(highdesc)) BUT_High.Text = highdesc;

            if (!string.IsNullOrEmpty(lowdesc)) BUT_Low.Text = lowdesc;
        }

        private void BUT_Low_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0,
                        0, 0, 0))
                    TXT_rcchannel.BackColor = Color.Red;
                else
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex, Strings.ERROR);
            }
        }

        private void BUT_High_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 1, 0, 0,
                        0, 0, 0))
                    TXT_rcchannel.BackColor = Color.Green;
                else
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex, Strings.ERROR);
            }
        }

        private void BUT_Repeat_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0,
                        0, 0, 0))
                    TXT_rcchannel.BackColor = Color.Red;

                Application.DoEvents();
                Thread.Sleep(200);

                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 1, 0, 0,
                        0, 0, 0))
                    TXT_rcchannel.BackColor = Color.Green;

                Application.DoEvents();
                Thread.Sleep(200);

                if (MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_SET_RELAY, thisrelay, 0, 0, 0,
                        0, 0, 0))
                    TXT_rcchannel.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex, Strings.ERROR);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sourcectl = ((ContextMenuStrip)renameToolStripMenuItem.Owner).SourceControl;

            var desc = sourcectl.Text;
            InputBox.Show("Description", "Enter new Description", ref desc);
            sourcectl.Text = desc;

            if (sourcectl == BUT_High)
                Settings.Instance["Relay" + thisrelay + "_highdesc"] = desc;
            else if (sourcectl == BUT_Low)
                Settings.Instance["Relay" + thisrelay + "_lowdesc"] = desc;
            else if (sourcectl == TXT_rcchannel) Settings.Instance["Relay" + thisrelay + "_desc"] = desc;
        }
    }
}