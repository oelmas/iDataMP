﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace MissionPlanner.ArduPilot
{
    public class RetryTimeout
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _memberName;
        private DateTime _timeOutDateTime = DateTime.MinValue;

        /// <summary>
        ///     Has the task completed
        /// </summary>
        public bool Complete;

        /// <summary>
        ///     Number of total retrys
        /// </summary>
        public int Retries = 3;

        /// <summary>
        ///     Current retry count
        /// </summary>
        public int RetriesCurrent;

        /// <summary>
        ///     timeout between retrys
        /// </summary>
        public int TimeoutMS = 1000;

        /// <summary>
        ///     Action to do on retry
        /// </summary>
        public Action WorkToDo;

        public RetryTimeout(int Retrys = 30, int TimeoutMS = 1000, [CallerMemberName] string memberName = "")
        {
            Retries = Retrys;
            this.TimeoutMS = TimeoutMS;
            _memberName = memberName;
        }

        public DateTime TimeOutDateTime
        {
            get
            {
                lock (this)
                {
                    return _timeOutDateTime;
                }
            }
            set
            {
                lock (this)
                {
                    _timeOutDateTime = value;
                }
            }
        }

        public bool DoWork()
        {
            if (WorkToDo == null)
                throw new ArgumentNullException("WorkToDo");
            return Task.Run(() =>
            {
                Complete = false;
                for (RetriesCurrent = 0; RetriesCurrent < Retries; RetriesCurrent++)
                {
                    log.InfoFormat($"Retry {_memberName} {0} - {1}", RetriesCurrent,
                        TimeOutDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    WorkToDo();
                    TimeOutDateTime = DateTime.Now.AddMilliseconds(TimeoutMS);
                    while (DateTime.Now < TimeOutDateTime)
                    {
                        if (Complete)
                            return true;
                        Thread.Sleep(100);
                        log.Debug("TimeOutDateTime " + TimeOutDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
                    }
                }

                return false;
            }).GetAwaiter().GetResult();
        }

        public void ResetTimeout()
        {
            TimeOutDateTime = DateTime.Now.AddMilliseconds(TimeoutMS);
        }
    }
}