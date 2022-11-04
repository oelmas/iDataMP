using System;
using MissionPlanner.Comms;

namespace MissionPlanner.Antenna
{
    public class Maestro : ITrackerOutput
    {
        private const byte SetTarget = 0x84;
        private const byte SetSpeed = 0x87;
        private const byte SetAccel = 0x89;
        private const byte GetPos = 0x90;
        private const byte GetState = 0x93;
        private const byte GetErrors = 0xA1;
        private const byte GoHome = 0xA2;

        private int _panreverse = 1;
        private int _tiltreverse = 1;

        private readonly byte PanAddress = 0;
        private readonly byte TiltAddress = 1;
        public SerialPort ComPort { get; set; }

        /// <summary>
        ///     0-360
        /// </summary>
        public double TrimPan { get; set; }

        /// <summary>
        ///     -90 - 90
        /// </summary>
        public double TrimTilt { get; set; }

        public int PanStartRange { get; set; }
        public int TiltStartRange { get; set; }
        public int PanEndRange { get; set; }
        public int TiltEndRange { get; set; }
        public int PanPWMRange { get; set; }
        public int TiltPWMRange { get; set; }
        public int PanPWMCenter { get; set; }
        public int TiltPWMCenter { get; set; }
        public int PanSpeed { get; set; }
        public int TiltSpeed { get; set; }
        public int PanAccel { get; set; }
        public int TiltAccel { get; set; }

        public bool PanReverse
        {
            get => _panreverse == -1;
            set => _panreverse = value ? -1 : 1;
        }

        public bool TiltReverse
        {
            get => _tiltreverse == -1;
            set => _tiltreverse = value ? -1 : 1;
        }

        public bool Init()
        {
            if (PanStartRange - PanEndRange == 0)
            {
                CustomMessageBox.Show(Strings.InvalidPanRange, Strings.ERROR);
                return false;
            }

            if (TiltStartRange - TiltEndRange == 0)
            {
                CustomMessageBox.Show(Strings.InvalidTiltRange, Strings.ERROR);
                return false;
            }

            try
            {
                ComPort.Open();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorConnecting + ex.Message, Strings.ERROR);
                return false;
            }

            return true;
        }

        public bool Setup()
        {
            // speed
            SendCompactMaestroCommand(SetSpeed, 0, PanAddress, PanSpeed);
            SendCompactMaestroCommand(SetSpeed, 0, TiltAddress, TiltSpeed);

            // accel
            SendCompactMaestroCommand(SetAccel, 0, PanAddress, PanAccel);
            SendCompactMaestroCommand(SetAccel, 0, TiltAddress, TiltAccel);

            //getCenterPWs();

            return true;
        }

        public bool Pan(double Angle)
        {
            double angleRange = Math.Abs(PanStartRange - PanEndRange);

            var pulseWidth = PanPWMRange / angleRange * wrap_180(Angle - TrimPan) * _panreverse +
                             PanPWMCenter;

            var target = Constrain(pulseWidth, PanPWMCenter - PanPWMRange / 2,
                PanPWMCenter + PanPWMRange / 2);
            target *= 4;

            SendCompactMaestroCommand(SetTarget, 0, PanAddress, target);
            return true;
        }

        public bool Tilt(double Angle)
        {
            double angleRange = Math.Abs(TiltStartRange - TiltEndRange);

            var pulseWidth = TiltPWMRange / angleRange * (Angle - TrimTilt) * _tiltreverse +
                             TiltPWMCenter;

            var target = Constrain(pulseWidth, TiltPWMCenter - TiltPWMRange / 2,
                TiltPWMCenter + TiltPWMRange / 2);
            target *= 4;

            SendCompactMaestroCommand(SetTarget, 0, TiltAddress, target);
            return true;
        }

        public bool PanAndTilt(double pan, double tilt)
        {
            // check if we are using 180 + 180 servos
            if (Math.Abs(TiltStartRange - TiltEndRange) > 120)
            {
                var target = wrap_180(pan - TrimPan);

                Console.WriteLine(target);

                // target > +-90
                if (Math.Abs(target) > 90)
                {
                    if (Tilt(180 - tilt) && Pan(target))
                        return true;
                }
                else
                {
                    if (Tilt(tilt) && Pan(pan))
                        return true;
                }
            }
            else
            {
                if (Tilt(tilt) && Pan(pan))
                    return true;
            }

            return false;
        }

        public bool Close()
        {
            try
            {
                ComPort.Close();
            }
            catch
            {
            }

            return true;
        }

        private void getCenterPWs()
        {
            byte[] buffer;
            // set all to home (center)
            SendCompactMaestroCommand(GoHome);

            while (SendCompactMaestroCommand(GetState, 1)[0] == 0x01)
            {
            }

            // get center position -- pan
            buffer = SendCompactMaestroCommand(GetPos, 2, PanAddress);
            PanPWMCenter = (buffer[1] << 8) | buffer[0];

            // get center position -- tilt
            buffer = SendCompactMaestroCommand(GetPos, 2, TiltAddress);
            TiltPWMCenter = (buffer[1] << 8) | buffer[0];
        }

        private double wrap_180(double input)
        {
            if (input > 180)
                return input - 360;
            if (input < -180)
                return input + 360;
            return input;
        }

        private short Constrain(double input, double min, double max)
        {
            if (input < min)
                return (short)min;
            if (input > max)
                return (short)max;
            return (short)input;
        }

        private byte[] SendCompactMaestroCommand(byte cmd, int respByteCount = 0, byte addr = 0xFF, int data = -1)
        {
            byte[] buffer;
            if (addr == 0xFF)
                buffer = new[] { cmd };
            else if (data < 0)
                buffer = new[] { cmd, addr };
            else
                buffer = new[] { cmd, addr, (byte)(data & 0x7F), (byte)((data >> 7) & 0x7F) };
            ComPort.DiscardInBuffer();
            ComPort.Write(buffer, 0, buffer.Length);
            if (respByteCount > 0)
            {
                buffer = new byte[respByteCount];
                while (ComPort.BytesToRead < respByteCount)
                {
                }

                ComPort.Read(buffer, 0, respByteCount);
            }

            return buffer;
        }
    }
}