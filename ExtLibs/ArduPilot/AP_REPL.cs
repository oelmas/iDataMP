using System;
using System.Linq;
using System.Text;
using System.Threading;
using MissionPlanner.ArduPilot.Mavlink;

namespace MissionPlanner.ArduPilot
{
    public class AP_REPL
    {
        private bool _active;

        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

        private readonly byte _compid;

        private readonly MAVFtp _mavftp;
        private readonly MAVLinkInterface _mavint;

        private int _outsize;
        private int _outskip;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private int _sessionwrite;

        private readonly byte _sysid;

        private Timer _timer;
        //handle_command_int_packet
        //MAV_CMD_SCRIPTING
        //SCRIPTING_CMD_REPL_START

        public string basepath = "repl/";

        public EventHandler<string> NewResponse;

        public AP_REPL(MAVLinkInterface mavint)
        {
            _mavint = mavint;
            _sysid = _mavint.MAV.sysid;
            _compid = _mavint.MAV.compid;
            _mavftp = new MAVFtp(_mavint, _sysid, _compid);
        }

        public void Start()
        {
            if (_mavint.doCommandInt(_mavint.MAV.sysid, _mavint.MAV.compid, MAVLink.MAV_CMD.SCRIPTING, 0, 0, 0, 0, 0, 0,
                    0))
            {
                _active = true;

                if (!_mavftp.kCmdListDirectory("/", new CancellationTokenSource())
                        .Any(a => a.Name == "repl"))
                    if (_mavftp.kCmdListDirectory("/APM/", new CancellationTokenSource())
                        .Any(a => a.Name == "repl"))
                    {
                        NewResponse?.Invoke(this, "Base dir changed to " + "/APM/repl/");
                        basepath = "/APM/repl/";
                    }

                _timer = new Timer(state =>
                {
                    _semaphore.Wait();
                    try
                    {
                        if (!_active)
                            return;

                        if (!_mavint.BaseStream.IsOpen)
                            return;

                        if (_mavftp.kCmdOpenFileRO(basepath + "out", out _outsize, new CancellationTokenSource()))
                        {
                            var stream = _mavftp.kCmdReadFile(basepath + "out", _outsize, _cancellation);

                            _mavftp.kCmdTerminateSession();

                            NewResponse?.Invoke(this,
                                Encoding.UTF8.GetString(stream.ToArray().Skip(_outskip).ToArray()));

                            _outskip = (int)stream.Length;
                        }

                        _mavftp.kCmdTerminateSession();
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }, this, 0, 1000);

                SendLine(@"""Hello World!!""" + "\n");
            }
            else
            {
                NewResponse?.Invoke(this, "Failed to start REPL");
            }
        }

        public void Stop()
        {
            _active = false;
            _timer?.Dispose();
            if (_mavint.doCommandInt(_mavint.MAV.sysid, _mavint.MAV.compid, MAVLink.MAV_CMD.SCRIPTING, 1, 0, 0, 0, 0, 0,
                    0))
            {
            }
            else
            {
                NewResponse?.Invoke(this, "Failed to stop REPL");
            }

            _cancellation.Cancel();
        }

        public void SendLine(string line)
        {
            SendLine(Encoding.UTF8.GetBytes(line));
        }

        public void SendLine(byte[] bytedata)
        {
            var createsize = 0;

            _semaphore.Wait();
            try
            {
                var list = _mavftp.kCmdListDirectory(basepath.TrimEnd('/'), new CancellationTokenSource());

                var useopen = list.Where(a => a.Name == "in");

                if ((useopen.Count() > 0 &&
                     _mavftp.kCmdOpenFileWO(basepath + "in", ref createsize, new CancellationTokenSource())) ||
                    _mavftp.kCmdCreateFile(basepath + "in", ref createsize, new CancellationTokenSource()))
                {
                    _mavftp.kCmdWriteFile(bytedata, (uint)(useopen.FirstOrDefault()?.Size ?? 0), "REPL", _cancellation);
                    _mavftp.kCmdTerminateSession();
                    _sessionwrite += bytedata.Length;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            SendLine(bytes.Skip(offset).Take(length).ToArray());
        }
    }
}