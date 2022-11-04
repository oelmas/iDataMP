namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services
{
    public interface IDatagramSender
    {
        string Host { get; set; }

        int PortNumber { get; set; }

        void SendToServer(byte[] data);
    }
}