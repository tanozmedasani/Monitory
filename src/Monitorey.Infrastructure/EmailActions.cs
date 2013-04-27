using monitory.Infrastructure.Interfaces;
using log4net;

namespace monitory.Infrastructure
{
    public class EmailActions : IEmailActions
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(EmailActions));
        readonly IApplicationSettings _applicationSettings;

        public EmailActions(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public void SendAlert(string message)
        {
            Log.Debug("Stepped into SendAlert");
           //build and send email or whatever alert here
        }
    }
}