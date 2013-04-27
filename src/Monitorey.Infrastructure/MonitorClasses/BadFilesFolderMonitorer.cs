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