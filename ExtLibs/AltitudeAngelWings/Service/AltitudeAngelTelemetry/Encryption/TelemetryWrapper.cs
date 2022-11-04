using System;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption
{
    public class TelemetryWrapper
    {
        public TelemetryWrapper(short messageType, byte[] message)
        {
            if (message.Length > short.MaxValue)
                throw new ArgumentException(string.Format(message.Length.ToString()),
                    nameof(message));

            var crc = Crc32.CalculateHash(message);
            Header = new TelemetryHeader(messageType, (short)message.Length, crc);
            Message = message;
        }

        public TelemetryWrapper(TelemetryHeader header, byte[] message)
        {
            Header = header;
            Message = message;
        }

        public TelemetryHeader Header { get; }

        public byte[] Message { get; }
    }
}