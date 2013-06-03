//Copyright [2012] [Jim Sowers]
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

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