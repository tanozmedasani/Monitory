using monitory.BusinessObjects;

namespace monitory.Infrastructure.Interfaces
{
    public interface IMonitorer
    {
        void Process(MonitorJob monitorJob);
    }
}