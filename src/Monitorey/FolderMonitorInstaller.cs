using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Reflection;

namespace monitory
{
    [RunInstaller(true)]
    public class FolderMonitorInstaller : Installer
    {
        System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller1;
        System.ServiceProcess.ServiceInstaller _serviceInstaller1;
        readonly Container _components = null;
        private readonly Configuration _config;
        private readonly string _serviceName;
        private readonly string _serviceDescription;


        public FolderMonitorInstaller()
        {
            Assembly service = Assembly.GetAssembly(typeof(MonitoryService));
            _config = ConfigurationManager.OpenExeConfiguration(service.Location);
            _serviceName = (_config.AppSettings.Settings["ServiceName"]).Value;
            _serviceDescription = (_config.AppSettings.Settings["ServiceDescription"]).Value;
            InitializeComponent();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        void InitializeComponent()
        {
            _serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            _serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // _serviceProcessInstaller1
            // 
            _serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            _serviceProcessInstaller1.Password = null;
            _serviceProcessInstaller1.Username = null;
            // 
            // _serviceInstaller1
            // 
            _serviceInstaller1.Description = _serviceDescription;
            _serviceInstaller1.DisplayName = _serviceName;
            _serviceInstaller1.ServiceName = _serviceName;
            // 
            // ProjectInstaller
            // 
            Installers.AddRange(new Installer[]
                {
                    _serviceProcessInstaller1,
                    _serviceInstaller1
                });
        }
    }
}