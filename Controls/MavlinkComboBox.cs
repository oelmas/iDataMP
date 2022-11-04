using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class MavlinkComboBox : ComboBox
    {
        private Type _source;
        private List<KeyValuePair<int, string>> _source2;
        private readonly string paramname2 = "";

        public MavlinkComboBox()
        {
            Enabled = false;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        [Browsable(true)] public string ParamName { get; set; }

        public Control SubControl { get; set; }

        public new event EventHandler SelectedIndexChanged;

        [Browsable(true)] public event EventHandler ValueUpdated;

        public void setup(string[] paramnames, MAVLink.MAVLinkParamList paramlist)
        {
            base.SelectedIndexChanged -= MavlinkComboBox_SelectedIndexChanged;

            var paramname = paramnames.FirstOrDefault(a => paramlist.ContainsKey(a));

            if (paramname != null)
            {
                var source =
                    ParameterMetaDataRepository.GetParameterOptionsInt(paramname,
                        MainV2.comPort.MAV.cs.firmware.ToString());

                _source2 = source;

                DisplayMember = "Value";
                ValueMember = "Key";
                DataSource = source;

                ParamName = paramname;
                Name = paramname;

                Enabled = true;
                Visible = true;

                enableControl(true);

                var item = paramlist[paramname];

                SelectedValue = (int)paramlist[paramname].Value;
            }

            base.SelectedIndexChanged += MavlinkComboBox_SelectedIndexChanged;
        }

        public void setup(List<KeyValuePair<int, string>> source, string paramname, MAVLink.MAVLinkParamList paramlist)
        {
            base.SelectedIndexChanged -= MavlinkComboBox_SelectedIndexChanged;

            _source2 = source;

            DisplayMember = "Value";
            ValueMember = "Key";
            DataSource = source;

            ParamName = paramname;
            Name = paramname;

            if (paramlist.ContainsKey(paramname))
            {
                Enabled = true;
                Visible = true;

                enableControl(true);

                var item = paramlist[paramname];

                SelectedValue = (int)paramlist[paramname].Value;
            }

            base.SelectedIndexChanged += MavlinkComboBox_SelectedIndexChanged;
        }


        public void setup(Type source, string paramname, MAVLink.MAVLinkParamList paramlist)
            //, string paramname2 = "", Control enabledisable = null)
        {
            base.SelectedIndexChanged -= MavlinkComboBox_SelectedIndexChanged;

            _source = source;

            DataSource = Enum.GetNames(source);

            ParamName = paramname;
            Name = paramname;

            if (paramlist.ContainsKey(paramname))
            {
                Enabled = true;
                Visible = true;

                enableControl(true);

                Text = Enum.GetName(source, (int)paramlist[paramname].Value);
            }

            base.SelectedIndexChanged += MavlinkComboBox_SelectedIndexChanged;
        }

        private void enableControl(bool enable)
        {
            if (SubControl != null)
                SubControl.Enabled = enable;
        }

        private void MavlinkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(sender, e);

            if (_source != null)
                try
                {
                    if (ValueUpdated != null)
                    {
                        ValueUpdated(this,
                            new MAVLinkParamChanged(ParamName, (int)Enum.Parse(_source, Text)));
                        return;
                    }

                    if (!MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                            ParamName, (float)(int)Enum.Parse(_source, Text)))
                        CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);

                    if (paramname2 != "")
                        if (
                            !MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                                (byte)MainV2.comPort.compidcurrent, paramname2,
                                (float)(int)Enum.Parse(_source, Text) > 0 ? 1 : 0))
                            CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, paramname2),
                                Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            else if (_source2 != null)
                try
                {
                    if (ValueUpdated != null)
                    {
                        ValueUpdated(this,
                            new MAVLinkParamChanged(ParamName, (int)((MavlinkComboBox)sender).SelectedValue));
                        return;
                    }

                    if (!MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                            ParamName, (float)(int)((MavlinkComboBox)sender).SelectedValue))
                        CustomMessageBox.Show("Set " + ParamName + " Failed!", Strings.ERROR);

                    if (paramname2 != "")
                        if (
                            !MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent,
                                (byte)MainV2.comPort.compidcurrent, paramname2,
                                (float)(int)((MavlinkComboBox)sender).SelectedValue > 0 ? 1 : 0))
                            CustomMessageBox.Show("Set " + paramname2 + " Failed!", Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show("Set " + ParamName + " Failed!", Strings.ERROR);
                }
        }
    }
}