using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace MissionPlanner
{
    public class MAVState : MAVLink, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<mavlink_camera_feedback_t> camerapoints = new List<mavlink_camera_feedback_t>();

        /// <summary>
        ///     the static global state of the currently connected MAV
        /// </summary>
        public CurrentState cs = new CurrentState();

        public ConcurrentDictionary<int, mavlink_mission_item_int_t> fencepoints =
            new ConcurrentDictionary<int, mavlink_mission_item_int_t>();

        /// <summary>
        ///     Store the guided mode wp location
        /// </summary>
        public mavlink_mission_item_int_t GuidedMode = new mavlink_mission_item_int_t();

        /// <summary>
        ///     mavlink 2 enable
        /// </summary>
        public bool mavlinkv2 = false;

        public DateTime packetlosttimer = DateTime.MinValue;

        private readonly object packetslock = new object();

        public float packetslost;
        public float packetsnotlost;

        /// <summary>
        ///     cache of all Types seen
        /// </summary>
        [JsonIgnore] [IgnoreDataMember]
        public ConcurrentDictionary<string, MAV_PARAM_TYPE> param_types =
            new ConcurrentDictionary<string, MAV_PARAM_TYPE>();

        [JsonIgnore] [IgnoreDataMember] public MAVLinkInterface parent;

        public Proximity Proximity;

        public ConcurrentDictionary<int, mavlink_mission_item_int_t> rallypoints =
            new ConcurrentDictionary<int, mavlink_mission_item_int_t>();

        internal int recvpacketcount;

        internal byte[] signingKey;
        public float synclost = 0;

        /// <summary>
        ///     used as a snapshot of what is loaded on the ap atm. - derived from the stream
        /// </summary>
        public ConcurrentDictionary<int, mavlink_mission_item_int_t> wps =
            new ConcurrentDictionary<int, mavlink_mission_item_int_t>();

        public MAVState(MAVLinkInterface mavLinkInterface, byte sysid, byte compid)
        {
            parent = mavLinkInterface;
            this.sysid = sysid;
            this.compid = compid;
            packetspersecond = new Dictionary<uint, double>(byte.MaxValue);
            packetspersecondbuild = new Dictionary<uint, DateTime>(byte.MaxValue);
            lastvalidpacket = DateTime.MinValue;
            sendlinkid = (byte)new Random().Next(256);
            signing = false;
            param = new MAVLinkParamList();
            var queuewrite = false;
            param.PropertyChanged += (s, a) =>
            {
                lock (param)
                {
                    if (queuewrite)
                        return;

                    queuewrite = true;
                }

                new Timer(o =>
                {
                    try
                    {
                        if (cs.uid2 == null || cs.uid2 == "" || sysid == 0)
                            return;
                        if (!Directory.Exists(Path.GetDirectoryName(ParamCachePath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(ParamCachePath));

                        lock (param)
                        {
                            File.WriteAllText(ParamCachePath, param.ToJSON());
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }

                    queuewrite = false;
                }, null, 2000, -1);
            };
            packets = new Dictionary<uint, Queue<MAVLinkMessage>>(byte.MaxValue);
            packetsLast = new Dictionary<uint, MAVLinkMessage>(byte.MaxValue);
            aptype = 0;
            apname = 0;
            recvpacketcount = 0;
            VersionString = "";
            SoftwareVersions = "";
            SerialString = "";
            FrameString = "";
            if (sysid != 255 && !(compid == 0 && sysid == 0)) // && !parent.logreadmode)
                Proximity = new Proximity(this, sysid, compid);

            camerapoints.Clear();

            packetslost = 0f;
            packetsnotlost = 0f;
            packetlosttimer = DateTime.MinValue;
            cs.parent = this;
        }

        public string ParamCachePath
        {
            get
            {
                try
                {
                    return Path.Combine(Settings.GetDataDirectory(), "paramcache",
                        aptype.ToString(),
                        cs.uid2,
                        sysid.ToString(),
                        compid.ToString(),
                        "param.json");
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

                return "";
            }
        }


        // all
        public string VersionString { get; set; }

        // px4+ only
        public string SoftwareVersions { get; set; }

        // px4+ only
        public string SerialString { get; set; }

        // AC frame type
        public string FrameString { get; set; }

        /// <summary>
        ///     mavlink remote sysid
        /// </summary>
        public byte sysid { get; set; }

        /// <summary>
        ///     mavlink remove compid
        /// </summary>
        public byte compid { get; set; }

        public byte linkid { get; set; }

        public byte sendlinkid { get; internal set; }

        public ulong timestamp { get; set; }

        /// <summary>
        ///     are we signing outgoing packets, and checking incomming packet signatures
        /// </summary>
        public bool signing { get; set; }

        /// <summary>
        ///     ignore the incomming signature
        /// </summary>
        public bool signingignore { get; set; }

        /// <summary>
        ///     storage for whole paramater list
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public MAVLinkParamList param { get; }

        /// <summary>
        ///     storage of a previous packet recevied of a specific type
        /// </summary>
        private Dictionary<uint, Queue<MAVLinkMessage>> packets { get; }

        /// <summary>
        ///     the last valid packet of this type.
        /// </summary>
        private Dictionary<uint, MAVLinkMessage> packetsLast { get; }

        /// <summary>
        ///     time seen of last mavlink packet
        /// </summary>
        public DateTime lastvalidpacket { get; set; }

        /// <summary>
        ///     used to calc packets per second on any single message type - used for stream rate comparaison
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<uint, double> packetspersecond { get; set; }

        /// <summary>
        ///     time last seen a packet of a type
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public Dictionary<uint, DateTime> packetspersecondbuild { get; set; }

        /// <summary>
        ///     mavlink ap type
        /// </summary>
        public MAV_TYPE aptype { get; set; }

        public MAV_AUTOPILOT apname { get; set; }

        public bool isHighLatency { get; set; }

        public bool CANNode { get; set; } = false;

        public ap_product Product_ID
        {
            get
            {
                if (param.ContainsKey("INS_PRODUCT_ID")) return (ap_product)(float)param["INS_PRODUCT_ID"];
                return ap_product.AP_PRODUCT_ID_NONE;
            }
        }

        public long time_offset_ns { get; set; }
        public CameraProtocol Camera { get; set; }
        public GimbalProtocol Gimbal { get; set; }

        public void Dispose()
        {
            if (Proximity != null)
                Proximity.Dispose();
        }

        public MAVLinkMessage getPacket(uint mavlinkid)
        {
            //log.InfoFormat("getPacket {0}", (MAVLINK_MSG_ID)mavlinkid);
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                    if (packets[mavlinkid].Count > 0)
                        return packets[mavlinkid].Dequeue();
            }

            return null;
        }

        public MAVLinkMessage getPacketLast(uint mavlinkid)
        {
            lock (packetslock)
            {
                if (packetsLast.ContainsKey(mavlinkid)) return packetsLast[mavlinkid];
            }

            return null;
        }

        public void addPacket(MAVLinkMessage msg)
        {
            lock (packetslock)
            {
                // create queue if it does not exist
                if (!packets.ContainsKey(msg.msgid)) packets[msg.msgid] = new Queue<MAVLinkMessage>();
                // cleanup queue if not polling this message
                while (packets[msg.msgid].Count > 5) packets[msg.msgid].Dequeue();

                packets[msg.msgid].Enqueue(msg);
                packetsLast[msg.msgid] = msg;
            }
        }

        public void clearPacket(uint mavlinkid)
        {
            lock (packetslock)
            {
                if (packets.ContainsKey(mavlinkid))
                {
                    packets[mavlinkid].Clear();
                    ;
                }
            }
        }

        public override string ToString()
        {
            return sysid.ToString();
        }
    }
}