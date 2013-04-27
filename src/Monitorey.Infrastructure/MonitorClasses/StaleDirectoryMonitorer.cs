using System;
using System.IO;
using monitory.BusinessObjects;
using monitory.Infrastructure.CustomExceptions;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure.MonitorClasses
{
    public class StaleDirectoryMonitorer : IMonitorer
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(StaleDirectoryMonitorer));
        readonly IEmailActions _emailActions;
        readonly ITimeActions _timeActions;

        public StaleDirectoryMonitorer(IEmailActions emailActions, ITimeActions timeActions)
        {
            _emailActions = emailActions;
            _timeActions = timeActions;
        }

        public void Process(MonitorJob monitorJob)
        {
            Log.DebugFormat("Stepped into process for monitorjob '{0}'", monitorJob.ID);
            if (!Directory.Exists(monitorJob.Path))
            {
                Log.ErrorFormat("We could not find the directory '{0}'", monitorJob.Path);
                throw new DirectoryNotFoundException(monitorJob.Path);
            }

            var lastWriteTime = Directory.GetLastWriteTime(monitorJob.Path);
            if (lastWriteTime < _timeActions.Now().Subtract(GetTimeSpanToUse(monitorJob.Threshold, monitorJob.ThresholdType)))
            {
                var emailMessage = string.Format("No new files have shown up in the directory '{0}' in the last '{1}' '{2}'", monitorJob.Path, monitorJob.Threshold, monitorJob.ThresholdType);
                Log.InfoFormat(emailMessage);
                _emailActions.SendAlert(emailMessage);
            }
        }

        internal TimeSpan GetTimeSpanToUse(int threshold, ThresholdType thresholdType)
        {
            if (thresholdType == ThresholdType.Seconds)
            {
                return TimeSpan.FromSeconds(threshold);
            }
            if (thresholdType == ThresholdType.Hours)
            {
                return TimeSpan.FromHours(threshold);
            }
            if (thresholdType == ThresholdType.Minutes)
            {
                return TimeSpan.FromMinutes(threshold);
            }
            
            throw new InvalidThresholdTypeException(thresholdType.ToString());
        }
    }
}