using monitory.BusinessObjects;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Tests
{
    [TestFixture]
    public class MonitorFactoryTests
    {
        IMonitorFactory _monitorFactory;
        IEmailActions _emailActions;
        ITimeActions _timeActions;

        [SetUp]
        public void SetUp()
        {
            _timeActions = MockRepository.GenerateStub<ITimeActions>();
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _monitorFactory = new MonitorFactory(_emailActions, _timeActions);
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_monitorFactory, Is.Not.Null);
        }

        [Test]
        public void MonitorFactoryReturnsABadFileMonitorerWhenItShould()
        {
            var result = _monitorFactory.GetMonitorer(new MonitorJob
                {
                    MontiredJobType = MontiredJobType.BadFileDirectory
                });

            Assert.That(result is BadFilesFolderMonitorer);
        }

        [Test]
        public void MonitorFactoryReturnsAFileSmallerThanThresholdMonitorerWhenItShould()
        {
            var result = _monitorFactory.GetMonitorer(new MonitorJob
                {
                    MontiredJobType = MontiredJobType.FileSmallerThanThreshold
                });

            Assert.That(result is FileSmallerThanThresholdMonitorer);
        }

        [Test]
        public void MonitorFactoryReturnsAStaleDirectoryMonitorerWhenItShould()
        {
            var result = _monitorFactory.GetMonitorer(new MonitorJob
                {
                    MontiredJobType = MontiredJobType.StaleDirectory
                });

            Assert.That(result is StaleDirectoryMonitorer);
        }

        [Test]
        public void MonitorFactoryReturnsAStaleFileMonitorerWhenItShould()
        {
            var result = _monitorFactory.GetMonitorer(new MonitorJob
                {
                    MontiredJobType = MontiredJobType.StaleFileMonitor
                });

            Assert.That(result is StaleFileMonitorer);
        }


    }
}
