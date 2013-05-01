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
    public class FileSmallerThanThresholdMonitorer : IMonitorer
    {
        static readonly ILog Log = LogManager.GetLogger(typeof (FileSmallerThanThresholdMonitorer));

        readonly IEmailActions _emailActions;

        public FileSmallerThanThresholdMonitorer(IEmailActions emailActions)
        {
            _emailActions = emailActions;
        }

        public void Process(MonitorJob monitorJob)
        {
            Log.DebugFormat("Stepped into process for monitorjob '{0}' with directory '{1}' and fileExtensionToWatch '{2}'", monitorJob.ID,monitorJob.Path, monitorJob.FileExtensionToWatch);
            if (!Directory.Exists(monitorJob.Path))
            {
                Log.ErrorFormat("We could not find the directory '{0}'", monitorJob.Path);
                throw new DirectoryNotFoundException(monitorJob.Path);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(monitorJob.Path);

            var allTheFiles = directoryInfo.GetFiles(monitorJob.FileExtensionToWatch);
            Log.DebugFormat("We found '{0}' files with extension '{1}'to check the size on in the directory '{2}'", allTheFiles.Length, monitorJob.FileExtensionToWatch,monitorJob.Path);
            foreach (var fileInfo in allTheFiles)
            {
                Log.DebugFormat("FileSmallerThanThresholdMonitorer found '{0}' files to process", allTheFiles.Length);
                if (fileInfo.Length < monitorJob.MinFileSizeInBytes)
                {
                    Log.DebugFormat("we are looking at a file of size '{0}'", fileInfo.Length);
                    var message = string.Format("There is a file '{0}' of type '{1}' smaller than the min filesize'{2}' in the directory '{3}'", fileInfo.Name, monitorJob.FileExtensionToWatch, monitorJob.MinFileSizeInBytes, monitorJob.Path);
                    Log.InfoFormat(message);
                    _emailActions.SendAlert(message);
                    return;
                }
            }
            Log.DebugFormat("FileSmallerThanThresholdMonitorer.Process did not find a file smaller than '{0}'bytes to process", monitorJob.MinFileSizeInBytes);
        }
    }
}