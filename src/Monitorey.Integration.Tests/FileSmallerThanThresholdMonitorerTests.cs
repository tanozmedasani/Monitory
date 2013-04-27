using monitory.BusinessObjects;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class FileSmallerThanThresholdMonitorerTests
    {
        FileSmallerThanThresholdMonitorer _fileSmallerThanThresholdMonitorer;
        IEmailActions _emailActions;

        [SetUp]
        public void SetUp()
        {
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _fileSmallerThanThresholdMonitorer = new FileSmallerThanThresholdMonitorer(_emailActions);
        }

        [Test]
        public void CanBeSetUp()
        {
            Assert.That(_fileSmallerThanThresholdMonitorer, Is.Not.Null);
        }

        [Test]
        public void CallsEmailActionsWhenThereIsAFileInTheFolderSmallerThanTheThresholdSize()
        {
            var monitorJobToUse = new MonitorJob {MontiredJobType = MontiredJobType.FileSmallerThanThreshold, Path = @"..\monitory.Integration.Tests\TestBadFileFolder", MinFileSizeInBytes = 999, FileExtensionToWatch = "*"};
            _fileSmallerThanThresholdMonitorer.Process(monitorJobToUse);
            var message = "There is a file 'badFile.txt' of type '*' smaller than the min filesize'999' in the directory '..\\monitory.Integration.Tests\\TestBadFileFolder'";
            _emailActions.AssertWasCalled(x => x.SendAlert(message));
        }

    }
}