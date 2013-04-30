using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure
{
    public class ApplicationSettings : IApplicationSettings
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationSettings));

        
        public ApplicationSettings()
        {
            LoadAllConfigValues();
        }

        public string ConnectionString { get; private set; }
        public int MinutesBetweenCheckingForNewMonitorJobs { get; private set; }
        public bool ShouldLoadJobsFromConfig { get; private set; }
        public int HourToStartMonitoring { get; private set; }
        public int HourToStopMonitoring { get; private set; }
        public int RetryIntervalInSeconds { get; private set; }
        public string Source { get;  private set; }
        public List<string> EmailToList { get; private set; }
        public string EmailFrom { get; private set; }
        
        public void LoadAllConfigValues()
        {
            try
            {
                ShouldLoadJobsFromConfig = bool.Parse(ConfigurationManager.AppSettings["ShouldLoadJobsFromConfig"]);
                ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                MinutesBetweenCheckingForNewMonitorJobs = Convert.ToInt32(ConfigurationManager.AppSettings["MinutesBetweenCheckingForNewMonitorJobs"]);
                HourToStartMonitoring = Convert.ToInt32(ConfigurationManager.AppSettings["HourToStartMonitoring"]);
                HourToStopMonitoring = Convert.ToInt32(ConfigurationManager.AppSettings["HourToStopMonitoring"]);
                RetryIntervalInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["RetryIntervalInSeconds"]);
                EmailToList = GetPipeDelimitedConfigValue(ConfigurationManager.AppSettings["EmailToList"]);
                EmailFrom = ConfigurationManager.AppSettings["EmailFrom"];
                Source = ConfigurationManager.AppSettings["Source"];

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("NasInbound Service was trying to LoadConfigValues and threw the error '{0}'", ex);
                throw;
            }
        }

        public List<string> GetPipeDelimitedConfigValue(string valueNameFromConfig)
        {
            string stringOfPipeDelimitedConfigValues = valueNameFromConfig;
            string[] ourList = stringOfPipeDelimitedConfigValues.Split('|');

            return ourList.ToList();
        } 



    }
}