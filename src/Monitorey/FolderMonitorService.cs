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
using System.ServiceProcess;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using monitory.Interfaces;
using log4net;
using log4net.Config;

namespace monitory
{
    public class MonitoryService : ServiceBase
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(MonitoryService));
        System.ComponentModel.Container _components;
        IThreadWrangler _thread;
        IDataActions _dataActions;
        IApplicationSettings _applicationSettings;
        IMonitorFactory _monitorFactory;
        IEmailActions _emailActions;
        IMonitorJobActions _monitorJobActions;
        ITimeActions _timeActions;

        public MonitoryService()
        {
            InitializeComponent();
            LoadAppSettings();
        }

        void LoadAppSettings()
        {
            try
            {
                _applicationSettings = new ApplicationSettings();
                _dataActions = new DataActions(_applicationSettings);
                _timeActions = new TimeActions();
                _emailActions = new EmailActions(_applicationSettings);
                _monitorJobActions = new MonitorJobActions(_timeActions,_applicationSettings);
                _monitorFactory = new MonitorFactory(_emailActions,_timeActions);

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("monitoryService was unable to create all the startup objects it needs to run. The exception was '{0}'", ex);
                throw;
            }
        }

        static void Main(string[] args)
        {
            //This configurator starts up log4net
            XmlConfigurator.Configure();
            
            //Add '/console' to start options > Command Line Arguments in your project properties to get the service to start in console for debugging
            if (args.Length > 0 && args[0] == "/console")
            {
                Log.Debug("Starting the monitory service in Console mode for debugging");
                var monitoryService = new MonitoryService();
                monitoryService.OnStart(args);
                Console.ReadKey();
            }
            else
            {
                var servicesToRun = new ServiceBase[] {new MonitoryService()};
                Run(servicesToRun);
            }
        }

        protected override void OnStart(string[] args)
        {
            _thread = new ThreadWrangler(_dataActions, _applicationSettings, _monitorFactory, _monitorJobActions);
            _thread.Start();
            Log.WarnFormat("The monitory successfully started");
        }

        protected override void OnStop()
        {
            try
            {
                Log.Fatal("Stopping The monitoryService. Bye");
                _thread.Stop();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("The monitoryService threw the error '{0}'", ex);
                Environment.Exit(ExitCode);
            }
        }

        private void InitializeComponent()
        {
            Log.DebugFormat("Calling InitializeComponent");
            _components = new System.ComponentModel.Container();
            ServiceName = "eScreen - monitoryService";
        }


        protected override void Dispose(bool disposing)
        {
            Log.DebugFormat("Calling dispose");
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
