//Copyright [2012] [Jim Sowers]
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using monitory.BusinessObjects;
using monitory.Infrastructure.CustomExceptions;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure
{
    public class MonitorJobActions : IMonitorJobActions
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(MonitorJobActions));
        
        readonly ITimeActions _timeActions;
        readonly IApplicationSettings _applicationSettings;

        public MonitorJobActions(ITimeActions timeActions, IApplicationSettings applicationSettings)
        {
            _timeActions = timeActions;
            _applicationSettings = applicationSettings;
        }

        public bool ThisJobShouldRunNow(MonitorJob monitorJob)
        {
            if (!monitorJob.LastTimeThisJobRan.HasValue)
            {
                Log.DebugFormat("ThisJobShouldRunNow is returning 'true' for monitorJobId '{0}'", monitorJob.ID);
                return true;
            }

            if (NextTimeThisJobShouldRun(monitorJob) < _timeActions.Now())
            {
                Log.DebugFormat("ThisJobShouldRunNow is returning 'true' for monitorJobId '{0}'", monitorJob.ID);
                return true;
            }
            Log.DebugFormat("ThisJobShouldRunNow is returning 'false' for monitorJobId '{0}'", monitorJob.ID);
            return false;
        }

        public DateTime NextTimeThisJobShouldRun(MonitorJob monitorJob)
        {
            var result =  monitorJob.LastTimeThisJobRan.Value.Add(GetTimeSpanToUse(monitorJob.Threshold, monitorJob.ThresholdType));
            Log.DebugFormat("NextTimeThisJobShouldRun is returning '{0}'", result);
            return result;
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

        public bool WeAreInTheRunWindow()
        {
            var result =  _timeActions.Now().Hour >= _applicationSettings.HourToStartMonitoring && _timeActions.Now().Hour < _applicationSettings.HourToStopMonitoring;
            Log.DebugFormat("WeAreInTheRunWindow is returning '{0}'", result);
            return result;
        }

        public bool JobsetHasExpired(MonitorJobSet monitorJobSet)
        {
            var timeThisJobsetShouldExpire = monitorJobSet.CreatedDate.AddMinutes(_applicationSettings.MinutesBetweenCheckingForNewMonitorJobs);
            var rightNow = _timeActions.Now();
            var result =  timeThisJobsetShouldExpire < rightNow;
            Log.DebugFormat("JobsetHasExpired is returning '{0}'", result);
            return result;
        }
 
    }
}