using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class StreamCombiner
    {
        private static readonly List<TcpClient> clients = new List<TcpClient>();

        private static readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 5750);

        private static TcpClient Server;

        public static Thread th;

        private static bool run;

        private static byte newsysid = 1;

        private static readonly object locker = new object();

        public static void Start()
        {
            if (run)
            {
                Stop();

                return;
            }

            newsysid = 1;

            listener.Start();

            listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);

            foreach (var portno in Range(5760, 10, 100))
            {
                var cl = new TcpClient();

                cl.BeginConnect(IPAddress.Loopback, portno, RequestCallback, cl);

                Thread.Sleep(100);
            }

            th = new Thread(mainloop)
            {
                IsBackground = true,
                Name = "stream combiner"
            };
            th.Start();

            //MainV2.comPort.BaseStream = new TcpSerial() {client = new TcpClient("127.0.0.1", 5750) };

            //MainV2.instance.doConnect(MainV2.comPort, "preset", "5750");
        }

        private static IEnumerable<int> Range(int start, int inc, int count)
        {
            var ans = new List<int>();

            for (var a = 0; a < count; a++) ans.Add(start + inc * a);

            return ans;
        }

        public static void Stop()
        {
            run = false;
            foreach (var client in clients)
                try
                {
                    client.Close();
                }
                catch
                {
                }

            clients.Clear();
        }

        private static void mainloop()
        {
            run = true;

            var buffer = new byte[1024];

            var mav = new MAVLink.MavlinkParse();

            while (run)
            {
                try
                {
                    if (Server == null)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    while (Server.Connected && Server.Available > 0)
                    {
                        var read = Server.GetStream().Read(buffer, 0, buffer.Length);

                        // write to all clients
                        foreach (var client in clients.ToArray())
                            if (client.Connected)
                                client.GetStream().Write(buffer, 0, read);
                    }
                }
                catch
                {
                }

                // read from all clients
                foreach (var client in clients.ToArray())
                    try
                    {
                        while (client.Connected && client.Available > 0)
                        {
                            var packet = mav.ReadPacket(client.GetStream());
                            if (packet == null)
                                continue;
                            if (Server != null && Server.Connected)
                                Server.GetStream().Write(packet.buffer, 0, packet.Length);
                        }
                    }
                    catch
                    {
                        client.Close();
                    }

                Thread.Sleep(1);
            }
        }

        private static void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            var listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            var client = listener.EndAcceptTcpClient(ar);

            Server = client;

            listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);
        }

        private static void RequestCallback(IAsyncResult ar)
        {
            var client = (TcpClient)ar.AsyncState;

            byte localsysid = 0;

            lock (locker)
            {
                localsysid = newsysid++;
            }

            if (client.Connected)
            {
                var mav = new MAVLinkInterface();

                mav.BaseStream = new TcpSerial { client = client };

                try
                {
                    mav.GetParam((byte)mav.sysidcurrent, (byte)mav.compidcurrent, "SYSID_THISMAV");
                }
                catch
                {
                }

                try
                {
                    mav.GetParam((byte)mav.sysidcurrent, (byte)mav.compidcurrent, "SYSID_THISMAV");
                }
                catch
                {
                }

                try
                {
                    mav.GetParam((byte)mav.sysidcurrent, (byte)mav.compidcurrent, "SYSID_THISMAV");
                }
                catch
                {
                }

                try
                {
                    var ans = mav.setParam("SYSID_THISMAV", localsysid);
                    Console.WriteLine("this mav set " + ans);
                }
                catch
                {
                }

                Connect?.Invoke(mav, localsysid.ToString());
            }
        }

        public static event Action<MAVLinkInterface, string> Connect;
    }
}