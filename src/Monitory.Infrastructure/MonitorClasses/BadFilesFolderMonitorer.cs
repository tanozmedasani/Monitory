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

using System.IO;
using monitory.BusinessObjects;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure.MonitorClasses
{
    public class BadFilesFolderMonitorer : IMonitorer
    {
        static readonly ILog Log = LogManager.GetLogger(typeof (BadFilesFolderMonitorer));

        readonly IEmailActions _emailActions;

        public BadFilesFolderMonitorer(IEmailActions emailActions)
        {
            _emailActions = emailActions;
        }

        public void Process(MonitorJob monitorJob)
        {
            Log.DebugFormat("Stepped into process for monitorjob '{0}'", monitorJob.ID);
            if (!Directory.Exists(monitorJob.Path))
            {
                Log.ErrorFormat("We could not find the BadFile directory '{0}'", monitorJob.Path);
                throw new DirectoryNotFoundException(monitorJob.Path);
            }

            string[] filesInBadFileDirectory = Directory.GetFiles(monitorJob.Path);
            
            if (filesInBadFileDirectory.Length > 0)
            {
                var message = string.Format("There are 'Bad Files' in the directory {0}", monitorJob.Path);
                Log.InfoFormat(message);
                _emailActions.SendAlert(message);
                
                return;
            }

            Log.DebugFormat("BadFilesFolderMonitorer did not find any files to gripe about in '{0}'", monitorJob.Path);
        }
    }
}