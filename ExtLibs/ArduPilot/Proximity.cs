﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using static MAVLink;

namespace MissionPlanner.Utilities
{
    public class Proximity : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MAVState _parent;
        private byte compid;

        private readonly int sub;
        private readonly int sub2;
        private byte sysid;

        public Proximity(MAVState mavInt, byte sysid, byte compid)
        {
            this.sysid = sysid;
            this.compid = compid;
            _parent = mavInt;
            sub = mavInt.parent.SubscribeToPacketType(MAVLINK_MSG_ID.DISTANCE_SENSOR, messageReceived, sysid,
                compid);

            sub2 = mavInt.parent.SubscribeToPacketType(MAVLINK_MSG_ID.OBSTACLE_DISTANCE, messageReceived, sysid,
                compid);

            log.InfoFormat("created for {0} - {1}", mavInt.sysid, mavInt.compid);
        }

        public directionState DirectionState { get; } = new directionState();

        public bool DataAvailable { get; set; }

        public void Dispose()
        {
            if (_parent != null)
                _parent.parent.UnSubscribeToPacketType(sub);
        }

        ~Proximity()
        {
            _parent?.parent?.UnSubscribeToPacketType(sub);
            _parent?.parent?.UnSubscribeToPacketType(sub2);
        }

        private bool messageReceived(MAVLinkMessage arg)
        {
            //accept any compid, but filter sysid
            if (arg.sysid != _parent.sysid)
                return true;

            if (arg.msgid == (uint)MAVLINK_MSG_ID.DISTANCE_SENSOR)
            {
                var dist = arg.ToStructure<mavlink_distance_sensor_t>();

                if (dist.current_distance >= dist.max_distance)
                    return true;

                if (dist.current_distance <= dist.min_distance)
                    return true;

                DirectionState.Add(dist.id, (MAV_SENSOR_ORIENTATION)dist.orientation, dist.current_distance,
                    DateTime.Now, 3);

                DataAvailable = true;
            }
            else if (arg.msgid == (uint)MAVLINK_MSG_ID.OBSTACLE_DISTANCE)
            {
                var dists = arg.ToStructure<mavlink_obstacle_distance_t>();

                var inc = dists.increment == 0 ? dists.increment_f : dists.increment;

                var rangestart = dists.angle_offset;

                if (dists.frame == (byte)MAV_FRAME.BODY_FRD)
                {
                    // no action
                }
                else if (dists.frame == (byte)MAV_FRAME.GLOBAL)
                {
                    rangestart += _parent.cs.yaw;
                }

                var rangeend = rangestart + inc * dists.distances.Length;

                for (var a = 0; a < dists.distances.Length; a++)
                {
                    // not used
                    if (dists.distances[a] == ushort.MaxValue)
                        continue;
                    if (dists.distances[a] > dists.max_distance)
                        continue;
                    if (dists.distances[a] < dists.min_distance)
                        continue;

                    var dist = Math.Min(Math.Max(dists.distances[a], dists.min_distance), dists.max_distance);
                    var angle = rangestart + inc * a;

                    DirectionState.Add(dists.sensor_type, angle, inc, dist, DateTime.Now, 0.2);
                }

                DataAvailable = true;
            }

            return true;
        }

        public class directionState
        {
            private readonly List<data> _dists = new List<data>();

            public void Add(uint id, MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received,
                double age = 1)
            {
                var existing = _dists.Where(a => { return a.SensorId == id && a.Orientation == orientation; });

                foreach (var item in existing.ToList()) _dists.Remove(item);

                _dists.Add(new data(id, orientation, distance, received, age));

                expire();
            }

            public void Add(uint id, double angle, double size, double distance, DateTime received, double age = 1)
            {
                var existing = _dists.Where(a => { return a.SensorId == id && a.Angle == angle; });

                foreach (var item in existing.ToList()) _dists.Remove(item);

                _dists.Add(new data(id, angle, size, distance, received, age));

                expire();
            }

            /// <summary>
            ///     Closest distance
            /// </summary>
            /// <returns></returns>
            public double GetClosest()
            {
                expire();

                var min = double.MaxValue;

                for (var a = 0; a < _dists.Count; a++) min = Math.Min(min, _dists[a].Distance);

                return min;
            }

            /// <summary>
            ///     List of direction bellow the min_distance
            /// </summary>
            /// <param name="min_distance"></param>
            /// <returns>List of directions</returns>
            public List<MAV_SENSOR_ORIENTATION> GetWarnings(double min_distance = 2)
            {
                expire();

                var list = new List<MAV_SENSOR_ORIENTATION>();

                for (var a = 0; a < _dists.Count; a++)
                    if (_dists[a].Distance < min_distance)
                        list.Add(_dists[a].Orientation);

                return list;
            }

            public List<data> GetRaw()
            {
                expire();

                return _dists;
            }

            private void expire()
            {
                lock (this)
                {
                    for (var a = 0; a < _dists.Count; a++)
                    {
                        var expireat = _dists[a].ExpireTime;

                        if (expireat < DateTime.Now)
                        {
                            // remove it
                            _dists.RemoveAt(a);
                            // make sure we dont skip an element
                            a--;
                            // move on
                        }
                    }
                }
            }

            public class data
            {
                public double Age;
                public double Angle;
                public double Distance;
                public MAV_SENSOR_ORIENTATION Orientation;
                public DateTime Received;
                public uint SensorId;
                public double Size;

                public data(uint id, MAV_SENSOR_ORIENTATION orientation, double distance, DateTime received,
                    double age = 1)
                {
                    SensorId = id;
                    Orientation = orientation;
                    Size = 45;
                    Distance = distance;
                    Received = received;
                    Age = age;
                }

                public data(uint id, double angle, double size, double distance, DateTime received, double age = 1)
                {
                    SensorId = id;
                    Orientation = MAV_SENSOR_ORIENTATION.MAV_SENSOR_ROTATION_CUSTOM;
                    Angle = angle;
                    Size = size;
                    Distance = distance;
                    Received = received;
                    Age = age;
                }

                public DateTime ExpireTime => Received.AddSeconds(Age);
            }
        }
    }
}