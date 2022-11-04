using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class MavlinkNumericUpDown : NumericUpDown
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private Control _control;
        private float _scale = 1;

        private readonly Timer timer = new Timer();

        public MavlinkNumericUpDown()
        {
            Min = 0;
            Max = 1;

            Name = "MavlinkNumericUpDown";

            timer.Tick += Timer_Tick;

            Enabled = false;
        }

        [Browsable(true)] public float Min { get; set; }

        [Browsable(true)] public float Max { get; set; }

        [Browsable(true)] public string ParamName { get; set; }

        [Browsable(true)] public event EventHandler ValueUpdated;

        public void setup(float Min, float Max, float Scale, float Increment, string paramname,
            MAVLink.MAVLinkParamList paramlist, Control enabledisable = null)
        {
            setup(Min, Max, Scale, Increment, new[] { paramname }, paramlist, enabledisable);
        }

        public void setup(float Min, float Max, float Scale, float Increment, string[] paramname,
            MAVLink.MAVLinkParamList paramlist, Control enabledisable = null)
        {
            ValueChanged -= MavlinkNumericUpDown_ValueChanged;

            // default to first item
            ParamName = paramname[0];
            // set a new item is first item doesnt exist
            foreach (var paramn in paramname)
                if (paramlist.ContainsKey(paramn))
                {
                    ParamName = paramn;
                    break;
                }

            // update local name
            Name = ParamName;
            // set min and max of both are equal
            double mint = Min, maxt = Max;
            if (ParameterMetaDataRepository.GetParameterRange(ParamName, ref mint, ref maxt,
                    MainV2.comPort.MAV.cs.firmware.ToString()))
            {
                Min = (float)mint;
                Max = (float)maxt;
            }

            if (Min == Max)
                log.InfoFormat("{0} {1} = {2}", ParamName, Min, Max);

            double Inc = 0;
            if (ParameterMetaDataRepository.GetParameterIncrement(ParamName, ref Inc,
                    MainV2.comPort.MAV.cs.firmware.ToString()))
                if (Inc > DecimalPlaces)
                    Increment = (float)Inc;

            _scale = Scale;
            Minimum = (decimal)Min;
            Maximum = (decimal)Max;
            this.Increment = (decimal)Increment;
            DecimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)Increment)[3])[2];

            _control = enabledisable;

            if (paramlist.ContainsKey(ParamName))
            {
                Enabled = true;
                Visible = true;

                enableControl(true);

                var value = (decimal)((float)paramlist[ParamName] / _scale);

                int dec = BitConverter.GetBytes(decimal.GetBits(value)[3])[2];

                if (dec > DecimalPlaces)
                    DecimalPlaces = dec;

                if (value < Minimum)
                    Minimum = value;
                if (value > Maximum)
                    Maximum = value;

                Value = value;
            }
            else
            {
                Enabled = false;
                enableControl(false);
            }

            ValueChanged += MavlinkNumericUpDown_ValueChanged;
        }

        private void enableControl(bool enable)
        {
            if (_control != null)
            {
                _control.Enabled = enable;
                _control.Visible = true;
            }
        }

        private void MavlinkNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var value = base.Text;
            if (decimal.Parse(value) > Maximum)
                if (
                    CustomMessageBox.Show(ParamName + " Value out of range\nDo you want to accept the new value?",
                        "Out of range", MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                {
                    Maximum = decimal.Parse(value);
                    Value = decimal.Parse(value);
                }

            if (ValueUpdated != null)
            {
                UpdateEditText();
                ValueUpdated(this, new MAVLinkParamChanged(ParamName, (float)Value * _scale));
                return;
            }

            lock (timer)
            {
                timer.Interval = 300;

                if (!timer.Enabled)
                    timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (timer)
            {
                try
                {
                    var ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, ParamName, (float)Value * _scale);
                    if (ans == false)
                        CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }

                timer.Stop();
            }
        }
    }
}