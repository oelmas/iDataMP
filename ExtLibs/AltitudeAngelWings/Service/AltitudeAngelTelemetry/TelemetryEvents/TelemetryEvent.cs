using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents
{
    /// <summary>
    ///     Telemetry event v1
    /// </summary>
    [Serializable]
    public class TelemetryEvent : ITelemetryEvent
    {
        public TelemetryEvent(Guid telemetryId, ITelemetryMessage message)
        {
            TelemetryId = telemetryId;
            Message = message;
        }

        /// <summary>
        ///     The id of one telemetry source which represents the data produced by an actor in this flight
        /// </summary>
        public Guid TelemetryId { get; }

        /// <summary>
        ///     Sequence number incremented by 1 for each item sent
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        ///     Type of <see cref="ITelemetryMessage" />
        /// </summary>
        public string MessageType => Message.MessageType;

        /// <summary>
        ///     Telemetry message information
        /// </summary>
        public ITelemetryMessage Message { get; }

        /// <inheritdoc />
        public int Version => 1;

        /// <summary>
        ///     Timestamp event was received in the server
        /// </summary>
        public DateTime ReceivedTimestamp { get; set; }
    }

    public class TelemetryEvent<T> : TelemetryEvent where T : ITelemetryMessage
    {
        public TelemetryEvent(Guid telemetryId, T message) :
            base(telemetryId, message)
        {
        }


        public new T Message => (T)base.Message;
    }
}