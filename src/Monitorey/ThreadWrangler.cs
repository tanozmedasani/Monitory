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
using System.Data.SqlClient;
using System.Threading;
using monitory.Infrastructure.Interfaces;
using monitory.Interfaces;
using log4net;

namespace monitory
{
    public class ThreadWrangler : IThreadWrangler
    {
        static readonly ILog Log = LogManager.GetLogger(typeof (ThreadWrangler));
        Thread _thread;
        readonly ManualResetEvent _manualResetEvent;
        bool _theServiceShouldContinue;
        readonly IDataActions _dataActions;
        readonly IApplicationSettings _applicationSettings;
        readonly IMonitorFactory _monitorFactory;
        readonly IMonitorJobActions _monitorJobActions;

        public ThreadWrangler(IDataActions dataActions, IApplicationSettings applicationSettings, IMonitorFactory monitorFactory, IMonitorJobActions monitorJobActions)
        {
            _dataActions = dataActions;
            _applicationSettings = applicationSettings;
            _monitorFactory = monitorFactory;
            _monitorJobActions = monitorJobActions;
            _manualResetEvent = new ManualResetEvent(false);
            _theServiceShouldContinue = true;
        }

        void ThreadStart()
        {
            Log.DebugFormat("Stepped into ThreadStart");
            var waitHandles = new WaitHandle[] {_manualResetEvent};
            _theServiceShouldContinue = true;

            var monitorJobSet = _dataActions.GetAllCurrentMonitorJobsForThisServer(Environment.MachineName);
            Log.DebugFormat("Threadstart found '{0}' jobs to run currently", monitorJobSet.MonitorJobs.Count);

            while (_theServiceShouldContinue)
            {
                try
                {
                    Log.DebugFormat("Looping");
                    if (_theServiceShouldContinue && _monitorJobActions.WeAreInTheRunWindow())
                    {
                        if (_monitorJobActions.JobsetHasExpired(monitorJobSet))
                        {
                            monitorJobSet = _dataActions.GetAllCurrentMonitorJobsForThisServer(Environment.MachineName);
                            Log.DebugFormat("Threadstart found '{0}' jobs to run currently", monitorJobSet.MonitorJobs.Count);
                        }

                        foreach (var monitorJob in monitorJobSet.MonitorJobs)
                        {
                            if (_monitorJobActions.ThisJobShouldRunNow(monitorJob))
                            {
                                var monitorer = _monitorFactory.GetMonitorer(monitorJob);
                                monitorer.Process(monitorJob);
                            }
                        }
                    }

                    if (WaitHandle.WaitAny(waitHandles, TimeSpan.FromSeconds(_applicationSettings.RetryIntervalInSeconds), false) != WaitHandle.WaitTimeout)
                    {
                        _theServiceShouldContinue = false;
                    }
                }
                catch (SqlException sqlEx)
                {
                    Log.ErrorFormat("ThreadStart caught a SqlException, database may be down, the exception was '{0}'", sqlEx);
                    //Do not throw here or the service will stop and we certainly do not want that.
                    if (WaitHandle.WaitAny(waitHandles, TimeSpan.FromMinutes(_applicationSettings.RetryIntervalInSeconds), false) != WaitHandle.WaitTimeout)
                    {
                        _theServiceShouldContinue = false;
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("ThreadStart threw the error '{0}'", ex);
                    //Do not throw here or the service will stop and we certainly do not want that.
                    if (WaitHandle.WaitAny(waitHandles, TimeSpan.FromMinutes(_applicationSettings.RetryIntervalInSeconds), false) != WaitHandle.WaitTimeout)
                    {
                        _theServiceShouldContinue = false;
                    }
                }
            }
        }


        public void Start()
        {
            Log.Debug("Stepped into Start");
            _manualResetEvent.Reset();
            _thread = new Thread(ThreadStart) {IsBackground = false};
            _thread.Start();
        }

        public void Stop()
        {
            Log.Fatal("Stepped into Stop");

            _theServiceShouldContinue = false;
            _manualResetEvent.Set();

            if (!_thread.Join(5000))
            {
                Log.Fatal("Aborting the thread.");
                _thread.Abort();
            }
        }
    }
}