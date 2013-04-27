using monitory.BusinessObjects;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class BadFileMonitorerTests
    {
        BadFilesFolderMonitorer _badFilesFolderMonitorer;
        IEmailActions _emailActions;

        [SetUp]
        public void SetUp()
        {
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _badFilesFolderMonitorer = new BadFilesFolderMonitorer(_emailActions);
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_badFilesFolderMonitorer, Is.Not.Null);
        }

        [Test]
        public void BadFileFolderMonitorerCallsEmailActionsWhenThereIsAFileOlderThanTheThreshold()
        {
            _badFilesFolderMonitorer.Process(new MonitorJob { MontiredJobType = MontiredJobType.BadFileDirectory, Path = @"..\monitory.Integration.Tests\TestBadFileFolder" });

            _emailActions.AssertWasCalled(x => x.SendAlert("There are 'Bad Files' in the directory ..\\monitory.Integration.Tests\\TestBadFileFolder"));
        }


    }
}
