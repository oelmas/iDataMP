using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;

namespace MissionPlanner.Comms
{
    /// <summary>
    ///     this is a proxy port for SERIAL_CONTROL messages
    /// </summary>
    public class MAVLinkSerialPort : ICommsSerial
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int subscription;

        private uint baud;

        private readonly Thread bgdata;

        private readonly CircularBuffer.CircularBuffer<byte> buffer = new CircularBuffer.CircularBuffer<byte>(4096);

        private DateTime lastgetdata = DateTime.MinValue;

        private readonly MAVLinkInterface mavint;

        private int packetcount;

        private DateTime packetcounttimer = DateTime.MinValue;
        private int packetwithdata;

        public MAVLink.SERIAL_CONTROL_DEV port = MAVLink.SERIAL_CONTROL_DEV.TELEM1;

        public ushort timeout = 10;

        public MAVLinkSerialPort(MAVLinkInterface mavint, int port)
            : this(mavint, (MAVLink.SERIAL_CONTROL_DEV)port)
        {
        }

        public MAVLinkSerialPort(MAVLinkInterface mavint, MAVLink.SERIAL_CONTROL_DEV port)
        {
            this.mavint = mavint;
            this.port = port;

            if (!mavint.BaseStream.IsOpen) throw new Exception(Strings.PleaseConnect);

            if (mavint.getHeartBeat().Length == 0) throw new Exception(Strings.No_valid_heartbeats_read_from_port);

            if (subscription != 0)
                mavint.UnSubscribeToPacketType(subscription);

            subscription = mavint.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SERIAL_CONTROL, ReceviedPacket,
                (byte)mavint.sysidcurrent, (byte)mavint.compidcurrent, true);

            bgdata = new Thread(mainloop);
            bgdata.Name = "MAVLinkSerialPort";
            bgdata.IsBackground = true;
            bgdata.Start();
        }

        public Parity Parity { get; set; }

        public StopBits StopBits { get; set; }

        public int BaudRate
        {
            set
            {
                if (IsOpen)
                {
                    log.Info("MAVLinkSerialPort baudrate " + value);
                    mavint.SendSerialControl(port, timeout, null, (uint)value);
                }

                baud = (uint)value;
            }
            get => (int)baud;
        }

        public int Read(byte[] readto, int offset, int length)
        {
            var count = 0;

            var deadline = DateTime.Now.AddMilliseconds(timeout);

            while (buffer.Size == 0)
            {
                GetData();
                Thread.Sleep(1);
                if (DateTime.Now > deadline)
                    throw new Exception("MAVLinkSerialPort Timeout on read");
                count += 1;
            }

            lock (buffer)
            {
                return buffer.Get(readto, offset, length);
            }
        }

        public void Write(byte[] write, int offset, int length)
        {
            var data = new byte[length];

            Array.Copy(write, offset, data, 0, length);

            mavint.SendSerialControl(port, 0, data);
        }

        public void Close()
        {
            IsOpen = false;

            if (bgdata.IsAlive)
                bgdata.Abort();

            log.Info("Close");
            mavint.SendSerialControl(port, 0, null, 0, true);
        }

        public void DiscardInBuffer()
        {
            buffer.Clear();
        }

        public void Open()
        {
            log.Info("Open");
            mavint.SendSerialControl(port, timeout, null, baud);
            Thread.Sleep(1000);
            IsOpen = true;
        }

        public int ReadByte()
        {
            var buffer = new byte[1];
            if (Read(buffer, 0, 1) > 0)
                return buffer[0];
            return -1;
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            var data = new byte[buffer.Size];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            var line = Encoding.UTF8.GetString(data, 0, data.Length);

            return line;
        }

        public string ReadLine()
        {
            var sb = new StringBuilder();

            char cha;

            do
            {
                cha = (char)ReadByte();
                sb.Append(cha);
            } while (cha != '\n');

            return sb.ToString();
        }

        public void Write(string text)
        {
            Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
        }

        public void WriteLine(string text)
        {
            Write(text + "\r\n");
        }

        public void toggleDTR()
        {
        }

        public void Dispose()
        {
            Close();
        }

        public Stream BaseStream => throw new NotImplementedException();

        public int BytesToRead
        {
            get
            {
                GetData();
                lock (buffer)
                {
                    return buffer.Size;
                }
            }
        }

        public int BytesToWrite => mavint.BaseStream.BytesToWrite;

        public int DataBits { get; set; }

        public bool DtrEnable { get; set; }

        public bool IsOpen { get; private set; }

        public string PortName { get; set; }

        public int ReadBufferSize
        {
            get => buffer.Capacity;
            set => buffer.Capacity = value;
        }

        public int ReadTimeout { get; set; }

        public bool RtsEnable { get; set; }

        public int WriteBufferSize { get; set; }

        public int WriteTimeout { get; set; }

        private void mainloop(object obj)
        {
            try
            {
                while (true)
                {
                    GetData();
                    Thread.Sleep(5);
                }
            }
            catch
            {
            }
        }

        ~MAVLinkSerialPort()
        {
            log.Info("Destroy");

            if (bgdata != null && bgdata.IsAlive)
                bgdata.Abort();
            //mavint.UnSubscribeToPacketType(subscription);
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (packetcounttimer.Second != DateTime.Now.Second)
            {
                log.Info("packet count " + packetcount + " with data " + packetwithdata + " " + buffer.Size);
                packetcount = 0;
                packetwithdata = 0;
                packetcounttimer = DateTime.Now;
            }

            packetcount++;

            var item = packet.ToStructure<MAVLink.mavlink_serial_control_t>();

            if (item.count == 0)
                return true;

            packetwithdata++;

            Console.WriteLine(DateTime.Now.Millisecond + "data count " + item.count);
            // ASCIIEncoding.UTF8.GetString(item.data, 0, item.count)

            lock (buffer)
            {
                buffer.AllowOverflow = true;
                buffer.Put(item.data, 0, item.count);
            }

            // we just received data, so do a request again
            GetData(true);

            return true;
        }

        private void GetData(bool now = false)
        {
            if ((lastgetdata.AddMilliseconds(timeout) < DateTime.Now && IsOpen) || now)
            {
                mavint.SendSerialControl(port, timeout, null);
                lastgetdata = DateTime.Now;
            }
        }
    }
}