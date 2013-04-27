using System;
using monitory.Infrastructure.Interfaces;

namespace monitory.Infrastructure
{
    public class TimeActions : ITimeActions
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        
    }
}