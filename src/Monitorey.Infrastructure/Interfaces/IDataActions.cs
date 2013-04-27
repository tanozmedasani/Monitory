using monitory.BusinessObjects;

namespace monitory.Infrastructure.Interfaces
{
    public interface IDataActions
    {
        MonitorJobSet GetAllCurrentMonitorJobsForThisServer(string machineName);
    }
}