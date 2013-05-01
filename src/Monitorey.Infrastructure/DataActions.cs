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
using System.Data;
using System.Data.SqlClient;
using monitory.BusinessObjects;
using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure
{
    public class DataActions : IDataActions
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(DataActions));
        readonly IApplicationSettings _applicationSettings;

        public DataActions(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public MonitorJobSet GetAllCurrentMonitorJobsForThisServer(string machineName)
        {
            Log.DebugFormat("Entering GetAllCurrentMonitorJobsForThisServer for machineName '{0}'", machineName);
            var monitorJobSet = new MonitorJobSet();
            var individualJobs = new List<MonitorJob>();
            
            using (var connection = new SqlConnection(_applicationSettings.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("usp_FolderMonitor_GetJobsByServerName", connection) { CommandType = CommandType.StoredProcedure })
                {
                    
                    try
                    {
                        command.Parameters.Add("@MachineName", SqlDbType.VarChar).Value = machineName;
                        
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                
                                var monitorJob = new MonitorJob
                                {
                                    ID = (int)reader["id"],
                                    MontiredJobType = (MontiredJobType)reader.GetInt32(Convert.ToInt32("MonitorJobType")),
                                    Path = reader["PathToMonitor"].ToString(),
                                    Threshold = reader["Threshold"] != null ? (int)reader["Threshold"] : 0,
                                    ThresholdType = reader["ThresholdType"] !=null ? (ThresholdType)((int)reader["ThresholdType"]) : ThresholdType.Minutes,
                                    FileExtensionToWatch = reader["FileExtensionToWatch"] == null ? "*" : reader["FileExtensionToWatch"].ToString(),
                                    MinFileSizeInBytes = reader["MinFileSizeInBytes"] != null ? (int)reader["MinFileSizeInBytes"] : int.MinValue,
                                };
                                individualJobs.Add(monitorJob);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("GetQueueItemsToProcess threw the exception '{0}' for machineName: '{1}'", ex, machineName);
                        throw;
                    }
                }
            }

            monitorJobSet.MonitorJobs = individualJobs;

            return monitorJobSet;
        }
    }
}