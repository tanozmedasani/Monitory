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
            var monitorJobToUse = new MonitorJob { MontiredJobType = MontiredJobType.StaleFileMonitor, Path = @"..\monitory.Integration.Tests\TestBadFileFolder",Threshold = 10, ThresholdType = ThresholdType.Seconds, FileExtensionToWatch = "*" };
            _staleFileMonitorer.Process(monitorJobToUse);
           var message = "There is a file 'badFile.txt' of type '*' older than the threshold '10' Seconds in the directory '..\\monitory.Integration.Tests\\TestBadFileFolder'";
            _emailActions.AssertWasCalled(x => x.SendAlert(message));
        }

        [Test]
        public void DoesNotCallEmailActionsWhenThereAreNoFilesInTheFolder()
        {
            _emailActions.Stub(x => x.SendAlert("This String Is Not Important ==> see ignore arguments...")).IgnoreArguments().Throw(new Exception());
            Directory.CreateDirectory("..\\monitory.Integration.Tests\\DirectoryThatWillBeGoneInASecond");
            var monitorJobToUse = new MonitorJob { MontiredJobType = MontiredJobType.StaleFileMonitor, Path = @"..\monitory.Integration.Tests\DirectoryThatWillBeGoneInASecond", Threshold = 10, ThresholdType = ThresholdType.Seconds, FileExtensionToWatch = "*" };
            _staleFileMonitorer.Process(monitorJobToUse);
            var messageThatWeShouldNeverGet = "There is a file 'badFile.txt' of type '*' older than the threshold '10' Seconds in the directory '..\\monitory.Integration.Tests\\DirectoryThatWillBeGoneInASecond'";
            _emailActions.AssertWasNotCalled(x => x.SendAlert(messageThatWeShouldNeverGet));
            Directory.Delete("..\\monitory.Integration.Tests\\DirectoryThatWillBeGoneInASecond");
        }

        

    }
}