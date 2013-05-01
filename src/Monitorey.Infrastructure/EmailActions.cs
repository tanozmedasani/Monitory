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
            Log.ErrorFormat("We Recieved the error meaages '{0}'", message);
           //build and send email or whatever alert here
        }
    }
}