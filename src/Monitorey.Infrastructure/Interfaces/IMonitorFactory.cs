using monitory.BusinessObjects;

namespace monitory.Infrastructure.Interfaces
{
    public interface IMonitorFactory
    {
        IMonitorer GetMonitorer(MonitorJob monitorJob);
    }
}