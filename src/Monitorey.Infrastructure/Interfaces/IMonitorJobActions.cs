using System;
using monitory.BusinessObjects;

namespace monitory.Infrastructure.Interfaces
{
    public interface IMonitorJobActions
    {
        bool ThisJobShouldRunNow(MonitorJob monitorJob);
        DateTime NextTimeThisJobShouldRun(MonitorJob monitorJob);
        bool WeAreInTheRunWindow();
        bool JobsetHasExpired(MonitorJobSet monitorJobSet);
    }
}