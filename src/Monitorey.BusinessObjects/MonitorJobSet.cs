using System;
using System.Collections.Generic;

namespace monitory.BusinessObjects
{
    public class MonitorJobSet
    {

        public MonitorJobSet()
        {
            CreatedDate = DateTime.Now;
        }

        public DateTime CreatedDate { get; private set; }
        public List<MonitorJob> MonitorJobs { get; set; }
        
    }
}