using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

namespace MissionPlanner.Joystick
{
    public class LinuxJoystickState : IMyJoystickState
    {
        private readonly Dictionary<byte, ushort> _jAxis;
        private readonly Dictionary<byte, bool> _jButton;

        public LinuxJoystickState(Dictionary<byte, ushort> jAxis, Dictionary<byte, bool> jButton)
        {
            _jAxis = jAxis;
            _jButton = jButton;

            for (byte a = 0; a < 128; a++)
                if (!_jButton.ContainsKey(a))
                    _jButton[a] = false;

            for (byte a = 0; a < 128; a++)
                if (!_jAxis.ContainsKey(a))
                    _jAxis[a] = 65535 / 2;
        }

        public int[] GetSlider()
        {
            return new[] { _jAxis[6], 65535 / 2 };
        }

        public int[] GetPointOfView()
        {
            return new[] { 65535 / 2 };
        }

        public bool[] GetButtons()
        {
            return _jButton.Values.ToArray();
        }

        public int AZ => _jAxis[7];

        public int AY => _jAxis[8];

        public int AX => _jAxis[9];

        public int ARz { get; }
        public int ARy { get; }
        public int ARx { get; }
        public int FRx { get; }
        public int FRy { get; }
        public int FRz { get; }
        public int FX { get; }
        public int FY { get; }
        public int FZ { get; }

        public int Rx => _jAxis[3];

        public int Ry => _jAxis[4];

        public int Rz => _jAxis[5];

        public int VRx { get; }
        public int VRy { get; }
        public int VRz { get; }
        public int VX { get; }
        public int VY { get; }
        public int VZ { get; }

        public int X => _jAxis[0];

        public int Y => _jAxis[1];

        public int Z => _jAxis[2];
    }

    public class WindowsJoystickState : IMyJoystickState
    {
        internal JoystickState baseJoystickState;

        public WindowsJoystickState(JoystickState state)
        {
            baseJoystickState = state;
        }

        public int[] GetSlider()
        {
            return baseJoystickState.Sliders;
        }

        public int[] GetPointOfView()
        {
            return baseJoystickState.PointOfViewControllers;
        }

        public bool[] GetButtons()
        {
            return baseJoystickState.Buttons;
        }

        public int AZ => baseJoystickState.AccelerationZ;

        public int AY => baseJoystickState.AccelerationY;

        public int AX => baseJoystickState.AccelerationX;

        public int ARz => baseJoystickState.AngularAccelerationZ;

        public int ARy => baseJoystickState.AngularAccelerationY;

        public int ARx => baseJoystickState.AngularAccelerationX;

        public int FRx => baseJoystickState.TorqueX;

        public int FRy => baseJoystickState.TorqueY;

        public int FRz => baseJoystickState.TorqueZ;

        public int FX => baseJoystickState.ForceX;

        public int FY => baseJoystickState.ForceY;

        public int FZ => baseJoystickState.ForceZ;

        public int Rx => baseJoystickState.RotationX;

        public int Ry => baseJoystickState.RotationY;

        public int Rz => baseJoystickState.RotationZ;

        public int VRx => baseJoystickState.AngularVelocityX;

        public int VRy => baseJoystickState.AngularVelocityY;

        public int VRz => baseJoystickState.AngularVelocityZ;

        public int VX => baseJoystickState.VelocityX;

        public int VY => baseJoystickState.VelocityY;

        public int VZ => baseJoystickState.VelocityZ;

        public int X => baseJoystickState.X;

        public int Y => baseJoystickState.Y;

        public int Z => baseJoystickState.Z;
    }
}