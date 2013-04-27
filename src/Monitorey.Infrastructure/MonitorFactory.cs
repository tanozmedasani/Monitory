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