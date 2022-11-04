using System;

namespace MissionPlanner.Controls
{
    public class MAVLinkParamChanged : EventArgs
    {
        public string name;
        public float value;

        public MAVLinkParamChanged(string Name, float Value)
        {
            name = Name;
            value = Value;
        }
    }
}