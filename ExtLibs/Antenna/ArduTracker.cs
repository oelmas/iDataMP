﻿using System;
using MissionPlanner.Comms;

namespace MissionPlanner.Antenna
{
    public class ArduTracker : ITrackerOutput
    {
        private int _panreverse = 1;
        private int _tiltreverse = 1;

        private int currentpan = 1500;
        private int currenttilt = 1500;
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
            get => _panreverse == 1;
            set => _panreverse = value ? -1 : 1;
        }

        public bool TiltReverse
        {
            get => _tiltreverse == 1;
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
            return true;
        }

        public bool Pan(double Angle)
        {
            double range = Math.Abs(PanStartRange - PanEndRange);

            // get relative center based on tracking center
            var rangeleft = PanStartRange - TrimPan;
            var rangeright = PanEndRange - TrimPan;
            double centerpos = PanPWMCenter;

            // get the output angle the tracker needs to point and constrain the output to the allowed options
            var PointAtAngle = Constrain(wrap_180(Angle - TrimPan), PanStartRange, PanEndRange);

            // conver the angle into a 0-pwmrange value
            var target = (int)(PointAtAngle / range * 2.0 * (PanPWMRange / 2) * _panreverse + centerpos);

            // Console.WriteLine("P " + Angle + " " + target + " " + PointAtAngle);

            currentpan = target;

            return false;
        }

        public bool Tilt(double Angle)
        {
            double range = Math.Abs(TiltStartRange - TiltEndRange);

            var PointAtAngle = Constrain(Angle - TrimTilt, TiltStartRange, TiltEndRange);

            var target = (int)(PointAtAngle / range * 2.0 * (TiltPWMRange / 2) * _tiltreverse + TiltPWMCenter);

            // Console.WriteLine("T " + Angle + " " + target + " " + PointAtAngle);

            currenttilt = target;

            return false;
        }

        public bool PanAndTilt(double pan, double tilt)
        {
            Tilt(tilt);
            Pan(pan);

            var command = string.Format("!!!PAN:{0:0000},TLT:{1:0000}\n", currentpan, currenttilt);

            Console.Write(command);

            ComPort.Write(command);

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

        private double wrap_180(double input)
        {
            if (input > 180)
                return input - 360;
            if (input < -180)
                return input + 360;
            return input;
        }

        private double wrap_range(double input, double range)
        {
            if (input > range)
                return input - 360;
            if (input < -range)
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
    }
}