using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MissionPlanner.Mavlink
{
    public class MAVList : IEnumerable<MAVState>, IDisposable
    {
        private readonly Dictionary<int, MAVState> hiddenlist = new Dictionary<int, MAVState>();

        private readonly object locker = new object();
        private readonly Dictionary<int, MAVState> masterlist = new Dictionary<int, MAVState>();

        public MAVLinkInterface parent;

        public MAVList(MAVLinkInterface mavLinkInterface)
        {
            parent = mavLinkInterface;
            // add blank item
            hiddenlist.Add(0, new MAVState(parent, 0, 0));
        }

        public MAVState this[int sysid, int compid]
        {
            get
            {
                var id = GetID((byte)sysid, (byte)compid);

                lock (locker)
                {
                    // 3dr radio special case
                    if (hiddenlist.ContainsKey(id))
                        return hiddenlist[id];

                    if (!masterlist.ContainsKey(id))
                    {
                        AddHiddenList((byte)sysid, (byte)compid);
                        return hiddenlist[id];
                    }

                    return masterlist[id];
                }
            }
            set
            {
                var id = GetID((byte)sysid, (byte)compid);
                lock (locker)
                {
                    masterlist[id] = value;
                }
            }
        }

        public int Count => masterlist.Count;

        public void Dispose()
        {
            lock (locker)
            {
                foreach (var MAV in hiddenlist) MAV.Value.Dispose();

                foreach (var MAV in masterlist) MAV.Value.Dispose();
            }
        }

        public IEnumerator<MAVState> GetEnumerator()
        {
            foreach (var key in masterlist.Values.ToArray()) yield return key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddHiddenList(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);
            lock (locker)
            {
                hiddenlist[id] = new MAVState(parent, sysid, compid);
            }
        }

        public List<int> GetRawIDS()
        {
            return masterlist.Keys.ToList();
        }

        public void Clear()
        {
            masterlist.Clear();
        }

        public bool Contains(byte sysid, byte compid, bool includehidden = true)
        {
            lock (locker)
            {
                foreach (var item in masterlist.ToArray())
                    if (item.Value.sysid == sysid && item.Value.compid == compid)
                        return true;

                if (includehidden)
                    foreach (var item in hiddenlist.ToArray())
                        if (item.Value.sysid == sysid && item.Value.compid == compid)
                            return true;
            }

            return false;
        }

        internal void Create(byte sysid, byte compid)
        {
            var id = GetID(sysid, compid);

            lock (locker)
            {
                // move from hidden to visible
                if (hiddenlist.ContainsKey(id))
                {
                    masterlist[id] = hiddenlist[id];
                    hiddenlist.Remove(id);
                }

                if (!masterlist.ContainsKey(id))
                    masterlist[id] = new MAVState(parent, sysid, compid);
            }
        }

        public static int GetID(byte sysid, byte compid)
        {
            return sysid * 256 + compid;
        }

        public static (byte sysid, byte compid) FromID(int id)
        {
            return ((byte)(id / 256), (byte)(id & 0xff));
        }
    }
}