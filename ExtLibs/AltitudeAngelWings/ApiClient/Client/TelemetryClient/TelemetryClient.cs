using System;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Encryption;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services;
using AltitudeAngelWings.Service.AltitudeAngelTelemetry.TelemetryEvents;

namespace AltitudeAngelWings.ApiClient.Client.TelemetryClient
{
    public class TelemetryClient : ITelemetryClient
    {
        private readonly IAutpService _autpService;

        public TelemetryClient(IAutpService autpService)
        {
            _autpService = autpService;
        }

        public void SendTelemetry(ITelemetryEvent dataStructure, string portName, int portNumber, string encryptionKey)
        {
            var bytes = _autpService.WriteTelemetry(dataStructure, Convert.FromBase64String(encryptionKey));
            IDatagramSender datagramSender = new DatagramSender(portName, portNumber);
            datagramSender.SendToServer(bytes);
        }
    }
}