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
using System.IO;
using monitory.BusinessObjects;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class StaleFileMonitorerTests
    {
        StaleFileMonitorer _staleFileMonitorer;
        IEmailActions _emailActions;
        ITimeActions _timeActions;
        IApplicationSettings _applicationSettings;

        [SetUp]
        public void SetUp()
        {
            _applicationSettings = MockRepository.GenerateStub<IApplicationSettings>();
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _timeActions = new TimeActions();
            _staleFileMonitorer = new StaleFileMonitorer(_emailActions, _timeActions);
        }
        
        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_staleFileMonitorer, Is.Not.Null);
        }

        [Test]
        public void CallsEmailActionsWhenThereIsAFileInTheFolderOlderThanTheThreshold()
        {
            var monitorJobToUse = new MonitorJob { MontiredJobType = MontiredJobType.StaleFileMonitor, Path = @"..\monitorey.Integration.Tests\TestBadFileFolder",Threshold = 10, ThresholdType = ThresholdType.Seconds, FileExtensionToWatch = "*" };
            _staleFileMonitorer.Process(monitorJobToUse);
           var message = "There is a file 'badFile.txt' of type '*' older than the threshold '10' Seconds in the directory '..\\monitorey.Integration.Tests\\TestBadFileFolder'";
            _emailActions.AssertWasCalled(x => x.SendAlert(message));
        }

        [Test]
        public void DoesNotCallEmailActionsWhenThereAreNoFilesInTheFolder()
        {
            _emailActions.Stub(x => x.SendAlert("This String Is Not Important ==> see ignore arguments...")).IgnoreArguments().Throw(new Exception());
            Directory.CreateDirectory("..\\monitorey.Integration.Tests\\DirectoryThatWillBeGoneInASecond");
            var monitorJobToUse = new MonitorJob { MontiredJobType = MontiredJobType.StaleFileMonitor, Path = @"..\monitorey.Integration.Tests\DirectoryThatWillBeGoneInASecond", Threshold = 10, ThresholdType = ThresholdType.Seconds, FileExtensionToWatch = "*" };
            _staleFileMonitorer.Process(monitorJobToUse);
            var messageThatWeShouldNeverGet = "There is a file 'badFile.txt' of type '*' older than the threshold '10' Seconds in the directory '..\\monitorey.Integration.Tests\\DirectoryThatWillBeGoneInASecond'";
            _emailActions.AssertWasNotCalled(x => x.SendAlert(messageThatWeShouldNeverGet));
            Directory.Delete("..\\monitory.Integration.Tests\\DirectoryThatWillBeGoneInASecond");
        }

        

    }
}