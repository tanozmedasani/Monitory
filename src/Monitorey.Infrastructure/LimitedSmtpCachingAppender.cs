using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net.Appender;
using log4net.Core;

namespace log4net.Extensions
{
    public class SmtpCachingAppender : SmtpAppender
    {
        //NOTE: see explanation of this class at http://handmadecode.com/2011/07/25/24/
        private readonly List<LoggingEvent> _loggingEvents = new List<LoggingEvent>();

        private int _numberOfCachedMessages;
        private bool _timeToFlushHasElapsed;
        private Timer _timer;

        public SmtpCachingAppender()
        {
            FlushIntervalTime = new TimeSpan(0, 5, 0);
            MinNumberToCacheBeforeSending = 20;
        }

        public TimeSpan FlushIntervalTime { get; set; }

        public int MinNumberToCacheBeforeSending { get; set; }

        public int MaxBufferSize { get; set; }

        public override void ActivateOptions()
        {
            if (FlushIntervalTime > TimeSpan.Zero)
            {
                _timer = new Timer(OnTimer, null, FlushIntervalTime, FlushIntervalTime);
            }
            base.ActivateOptions();
        }

        private void OnTimer(Object stateInfo)
        {
            _timeToFlushHasElapsed = true;
            SendBuffer();
        }

        private void SendBuffer()
        {
            try
            {
                if (MaxBufferSize != 0)
                {
                    int numRemoved = _loggingEvents.Count - MaxBufferSize;
                    if ((numRemoved > 0) && (numRemoved <= _loggingEvents.Count))
                    {
                        _loggingEvents.RemoveRange(0, numRemoved);
                    }
                }

                _numberOfCachedMessages++;

                if (((MinNumberToCacheBeforeSending != 0) && (_numberOfCachedMessages >= MinNumberToCacheBeforeSending)) ||
                    _timeToFlushHasElapsed)
                {
                    if (_loggingEvents.Count > 0)
                    {
                        LoggingEvent[] bufferedEvents = _loggingEvents.ToArray();

                        base.SendBuffer(bufferedEvents);

                        _loggingEvents.Clear();
                    }
                        // Reset cache buffer conditions.
                    _numberOfCachedMessages = 0;
                    _timeToFlushHasElapsed = false;
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            {
                //We never want to crash so we let nothing happen here purposefully
            }
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            foreach (var loggingEvent in events.Where(loggingEvent => !_loggingEvents.Contains(loggingEvent)))
            {
                _loggingEvents.Add(loggingEvent);
            }
            SendBuffer();
        }
    }
}