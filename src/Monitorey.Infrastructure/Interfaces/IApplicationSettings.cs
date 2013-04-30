using System.Collections.Generic;

namespace monitory.Infrastructure.Interfaces
{
    public interface IApplicationSettings
    {
        bool ShouldLoadJobsFromConfig { get; }
        int HourToStartMonitoring { get; }
        int HourToStopMonitoring { get; }
        int RetryIntervalInSeconds { get; }
        string Source { get; }
        List<string> EmailToList { get;  }
        string EmailFrom { get;  }
        string ConnectionString { get; }
        int MinutesBetweenCheckingForNewMonitorJobs { get;}
    }
}