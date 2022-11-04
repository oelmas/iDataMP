using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace AltitudeAngelWings.Service.AltitudeAngelTelemetry.Services
{
    public class DatagramSender : IDatagramSender
    {
        public DatagramSender(string hostName, int portNumber)
        {
            PortNumber = portNumber;
            Host = hostName;
        }

        public string IPAddress
        {
            get => IPAddressObject.ToString();
            set => Host = value;
        }

        private IPAddress IPAddressObject
        {
            get
            {
                return Dns.GetHostEntry(Host).AddressList
                    .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
        }

        private IPEndPoint IPEndPoint => new IPEndPoint(IPAddressObject, PortNumber);

        public string Host { get; set; }

        public int PortNumber { get; set; }

        public void SendToServer(byte[] data)
        {
            using (var client = new UdpClient(GetAvailablePort()))
            {
                client.Send(data, data.Length, IPEndPoint);
            }
        }

        /// <summary>
        ///     checks for used ports and retrieves the first free port
        /// </summary>
        /// <returns>the free port or 0 if it did not find a free port</returns>
        /// <remarks>https://gist.github.com/jrusbatch/4211535</remarks>
        private int GetAvailablePort(int startingPort = 0)
        {
            IPEndPoint[] endPoints;
            var portArray = new List<int>();

            var properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            var connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                where n.LocalEndPoint.Port >= startingPort
                select n.LocalEndPoint.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                where n.Port >= startingPort
                select n.Port);

            portArray.Sort();

            for (var i = startingPort; i < ushort.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }
    }
}