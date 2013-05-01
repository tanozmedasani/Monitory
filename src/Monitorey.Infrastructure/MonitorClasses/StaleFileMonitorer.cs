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
using System.IO;
using monitory.BusinessObjects;
using monitory.Infrastructure.CustomExceptions;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure.MonitorClasses
{
    public class StaleFileMonitorer : IMonitorer
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(StaleFileMonitorer));
        readonly IEmailActions _emailActions;
        readonly ITimeActions _timeActions;

        public StaleFileMonitorer(IEmailActions emailActions, ITimeActions timeActions)
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

            var directoryInfo = new DirectoryInfo(monitorJob.Path);

            var allTheFiles = directoryInfo.GetFiles(monitorJob.FileExtensionToWatch);

            foreach (var fileInfo in allTheFiles)
            {
                if (fileInfo.CreationTime < _timeActions.Now().Subtract(GetTimeSpanToUse(monitorJob.Threshold, monitorJob.ThresholdType)))
                
                {
                    var message = string.Format("There is a file '{0}' of type '{1}' older than the threshold '{2}' {3} in the directory '{4}'", 
                                                        fileInfo.Name, monitorJob.FileExtensionToWatch, monitorJob.Threshold, monitorJob.ThresholdType, monitorJob.Path);
                    Log.InfoFormat(message);
                    _emailActions.SendAlert(message);
                    return;
                }
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