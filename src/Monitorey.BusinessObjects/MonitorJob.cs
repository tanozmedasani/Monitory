using System;

namespace monitory.BusinessObjects
{
    public class MonitorJob
    {
        public int ID { get; set; }
        public MontiredJobType MontiredJobType { get; set; }
        public int Threshold { get; set; }
        public ThresholdType ThresholdType { get; set; }
        public string Path { get; set; }
        public int? MinFileSizeInBytes { get; set; }
        public string FileExtensionToWatch { get; set; } 
        public DateTime? LastTimeThisJobRan { get; set; }

        
    }
}
