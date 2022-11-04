using System;
using System.Runtime.InteropServices;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    ///     Position on a globe
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Position
    {
        public Position(float lat, float lon, ushort accuracy)
        {
            Lat = lat;
            Lon = lon;
            Accuracy = accuracy;
        }

        /// <summary>
        ///     Latitude
        /// </summary>
        public float Lat { get; }

        /// <summary>
        ///     Longitude
        /// </summary>
        public float Lon { get; }

        /// <summary>
        ///     Accuracy in meters
        /// </summary>
        public ushort Accuracy { get; }
    }
}