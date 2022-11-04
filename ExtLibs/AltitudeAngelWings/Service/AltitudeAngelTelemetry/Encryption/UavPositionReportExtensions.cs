﻿using System;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    internal static class UavPositionReportExtensions
    {
        private static readonly int PositionMultiplier = (int)Math.Pow(10, 7);
        private static readonly int VelocityMultiplier = (int)Math.Pow(10, 3);

        /// <summary>
        ///     Convert a <see cref="UavPositionReport" /> to an Autp <see cref="UavPosition" />
        /// </summary>
        /// <remarks>
        ///     Altitude should be specified in Wgs8, as that's what the UDP paket is expected to provide.
        ///     If it isn't the altitude will be inaccurate if it is serialized back to canonical again.
        ///     No conversion is made.
        /// </remarks>
        /// <returns>A UavPosition object</returns>
        public static UavPosition ToUavPosition(this UavPositionReport report)
        {
            var position = new UavPosition
            {
                // Position
                Latitude = (int)(report.Pos.Lat * PositionMultiplier),
                Longitude = (int)(report.Pos.Lon * PositionMultiplier),
                PositionAccuracy = report.Pos.Accuracy,
                IsAirborne = (byte)report.IsAirborne,

                // Altitude
                GpsAltitude = report.Alt.Value,
                AltitudeAccuracy = report.Alt.Accuracy,

                // Speed and heading
                EastVelocity = (int)(report.Velocity.X * VelocityMultiplier),
                NorthVelocity = (int)(report.Velocity.Y * VelocityMultiplier),
                UpVelocity = (int)(report.Velocity.Z * VelocityMultiplier),
                VelocityAccuracy = (int)(report.Velocity.Accuracy * VelocityMultiplier),

                Fuel = report.Fuel,
                SatellitesVisible = report.SatellitesVisible,
                GpsTimestamp = GpsTime.ToGpsTimestamp(report.GpsTimestamp),
                GpsTimestampOffset = (ushort)report.GpsTimestamp.Millisecond
            };
            return position;
        }

        /// <summary>
        ///     Convert an AUTP <see cref="UavPosition" /> to a <see cref="UavPositionReport" />
        /// </summary>
        /// <returns>A UavPositionReport object</returns>
        public static UavPositionReport ToPositionReport(this UavPosition position)
        {
            var report = new UavPositionReport
            {
                Alt = new Altitude(position.GpsAltitude, AltitudeDatum.Wgs84, position.AltitudeAccuracy),
                IsAirborne = Enum.IsDefined(typeof(AirborneStatus), position.IsAirborne)
                    ? (AirborneStatus)position.IsAirborne
                    : AirborneStatus.Unknown,
                Pos = new Position(
                    (float)position.Latitude / PositionMultiplier,
                    (float)position.Longitude / PositionMultiplier,
                    position.PositionAccuracy
                ),
                Velocity = new Velocity(
                    (float)position.EastVelocity / VelocityMultiplier,
                    (float)position.NorthVelocity / VelocityMultiplier,
                    (float)position.UpVelocity / VelocityMultiplier,
                    (float)position.VelocityAccuracy / VelocityMultiplier
                ),
                Fuel = position.Fuel,
                SatellitesVisible = position.SatellitesVisible,
                GpsTimestamp = GpsTime.ToDateTime(position.GpsTimestamp).AddMilliseconds(position.GpsTimestampOffset)
            };

            return report;
        }
    }
}