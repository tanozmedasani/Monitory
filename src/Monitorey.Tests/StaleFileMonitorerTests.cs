using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Tests
{
    [TestFixture]
    public class StaleFileMonitorerTests
    {
        StaleFileMonitorer _staleFileMonitorer;
        IEmailActions _emailActions;
        ITimeActions _timeActions;

        [SetUp]
        public void SetUp()
        {
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _timeActions = MockRepository.GenerateStub<ITimeActions>();
            _staleFileMonitorer = new StaleFileMonitorer(_emailActions, _timeActions);
        }
        
        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_staleFileMonitorer, Is.Not.Null);
        }

    }
}