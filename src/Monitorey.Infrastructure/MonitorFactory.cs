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

using monitory.BusinessObjects;
using monitory.Infrastructure.CustomExceptions;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;

namespace monitory.Infrastructure
{
    public class MonitorFactory : IMonitorFactory
    {
        readonly IEmailActions _emailActions;
        readonly ITimeActions _timeActions;

        public MonitorFactory(IEmailActions emailActions, ITimeActions timeActions)
        {
            _emailActions = emailActions;
            _timeActions = timeActions;
        }

        public IMonitorer GetMonitorer(MonitorJob monitorJob)
        {
            if (monitorJob.MontiredJobType == MontiredJobType.BadFileDirectory)
            {
                return new BadFilesFolderMonitorer(_emailActions);
            }

            if (monitorJob.MontiredJobType == MontiredJobType.FileSmallerThanThreshold)
            {
                return new FileSmallerThanThresholdMonitorer(_emailActions);
            }

            if (monitorJob.MontiredJobType == MontiredJobType.StaleDirectory)
            {
                return new StaleDirectoryMonitorer(_emailActions, _timeActions);
            }

            if (monitorJob.MontiredJobType == MontiredJobType.StaleFileMonitor)
            {
                return new StaleFileMonitorer(_emailActions, _timeActions);
            }

            throw new UnknownMonitorJobTypeException();

        }
    }
}