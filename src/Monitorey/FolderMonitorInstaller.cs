using System.ComponentModel;
using System.Configuration.Install;

namespace monitory
{
    [RunInstaller(true)]
    public class FolderMonitorInstaller : Installer
    {
        System.ServiceProcess.ServiceProcessInstaller _serviceProcessInstaller1;
        System.ServiceProcess.ServiceInstaller _serviceInstaller1;
        readonly Container _components = null;

        public FolderMonitorInstaller()
        {
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
            _serviceInstaller1.Description = "Watches folders for certain files or old files";
            _serviceInstaller1.DisplayName = "monitory - FolderMonitoringService";
            _serviceInstaller1.ServiceName = "monitory - FolderMonitoringService";
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