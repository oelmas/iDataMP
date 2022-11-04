using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class MavlinkCheckBox : CheckBox
    {
        private Control _control;

        private Action CallBackOnChange;

        public MavlinkCheckBox()
        {
            OnValue = 1;
            OffValue = 0;

            Enabled = false;
        }

        [Browsable(true)] public double OnValue { get; set; }

        [Browsable(true)] public double OffValue { get; set; }

        [Browsable(true)] public string ParamName { get; set; }

        public new event EventHandler CheckedChanged;

        public void setup(double[] OnValue, double[] OffValue, string[] paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null)
        {
            var idx = 0;
            foreach (var s in paramname)
            {
                if (paramlist.ContainsKey(s))
                {
                    setup(OnValue[idx], OffValue[idx], s, paramlist, enabledisable);
                    return;
                }

                idx++;
            }
        }

        public void setup(double OnValue, double OffValue, string[] paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null, Action callbackonchange = null)
        {
            foreach (var s in paramname)
                if (paramlist.ContainsKey(s))
                {
                    setup(OnValue, OffValue, s, paramlist, enabledisable, callbackonchange);
                    return;
                }
        }

        public void setup(double OnValue, double OffValue, string paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null, Action callbackonchange = null)
        {
            base.CheckedChanged -= MavlinkCheckBox_CheckedChanged;

            CallBackOnChange = callbackonchange;
            this.OnValue = OnValue;
            this.OffValue = OffValue;
            ParamName = paramname;
            _control = enabledisable;

            if (paramlist.ContainsKey(paramname))
            {
                Enabled = true;
                Visible = true;

                if (paramlist[paramname].Value == OnValue)
                {
                    Checked = true;
                    enableBGControl(true);
                }
                else if (paramlist[paramname].Value == OffValue)
                {
                    Checked = false;
                    enableBGControl(false);
                }
                else
                {
                    CheckState = CheckState.Indeterminate;
                    enableBGControl(false);
                }
            }
            else
            {
                Enabled = false;
            }

            base.CheckedChanged += MavlinkCheckBox_CheckedChanged;
        }

        private void enableBGControl(bool enable)
        {
            if (_control != null)
                _control.Enabled = enable;
        }

        private void MavlinkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(sender, e);

            if (Checked)
            {
                enableBGControl(true);
                try
                {
                    var ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, ParamName, OnValue);
                    if (ans == false)
                        CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                    else
                        CallBackOnChange?.Invoke();
                }
                catch
                {
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
            else
            {
                enableBGControl(false);
                try
                {
                    var ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, ParamName, OffValue);
                    if (ans == false)
                        CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                    else
                        CallBackOnChange?.Invoke();
                }
                catch
                {
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
        }
    }
}