using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    ///     Pilot position report
    /// </summary>
    [Serializable]
    public class PilotPositionReport : ITelemetryMessage
    {
        public Position Pos { get; set; }

        public string MessageType => MessageTypes.PilotPositionReport;

        public int Version => 1;
    }
}