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

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using monitory.BusinessObjects;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class StaleDirectoryMonitorerTests
    {
        StaleDirectoryMonitorer _staleDirectoryMonitorer;
        IEmailActions _emailActions;
        ITimeActions _timeActions;

        [SetUp]
        public void SetUp()
        {
            _timeActions = new TimeActions();
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _staleDirectoryMonitorer = new StaleDirectoryMonitorer(_emailActions, _timeActions);
            
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_staleDirectoryMonitorer, Is.Not.Null);
        }

        [Test]
        public void StaleDirectoryMonitorerCallsEmailActionsWhenThereIsAFileOlderThanTheThreshold()
        {
            var monitorJobToUse = new MonitorJob
                {
                    MontiredJobType = MontiredJobType.StaleDirectory,
                    Path = @"..\monitorey.Integration.Tests\TestStaleDirectory",
                    Threshold = -15,//NOTE: Negative on purpose so other tests that touch the directory don't make this one fail
                    ThresholdType = ThresholdType.Minutes,
                };

            _staleDirectoryMonitorer.Process(monitorJobToUse);

            _emailActions.AssertWasCalled(x => x.SendAlert("No new files have shown up in the directory '..\\monitorey.Integration.Tests\\TestStaleDirectory' in the last '-15' 'Minutes'"));
        }

        [Test]
        public void StaleDirectoryMonitorerDoesNotCallEmailActionsWhenThereWhenTheFolderHasBeenChangedInsideTheThreshold()
        {
            string path = @"..\Monitorey.Integration.Tests\TestStaleDirectory";
            string fileName = "testNewFile.txt";

            IdentityReference everybodyIdentity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var rule = new FileSystemAccessRule(
                everybodyIdentity,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);

            DirectorySecurity directorySecurity = Directory.GetAccessControl(path);
            directorySecurity.AddAccessRule(rule);
            Directory.SetAccessControl(path, directorySecurity);
            
            File.WriteAllText(Path.Combine(path, fileName), "content");

            _staleDirectoryMonitorer.Process(new MonitorJob
            {
                MontiredJobType = MontiredJobType.StaleDirectory,
                Path = path,
                Threshold = 1,
                ThresholdType = ThresholdType.Minutes,
            });

            File.Delete(Path.Combine(path, fileName));

            _emailActions.AssertWasNotCalled(x => x.SendAlert("No new files have shown up in the directory '..\\monitorey.Integration.Tests\\TestStaleDirectory' in the last '1' 'Minutes'"));

        }

    }
}