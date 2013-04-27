using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using NUnit.Framework;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class DataActionsTests
    {
        IApplicationSettings _applicationSettings;
        IDataActions _dataActions;

        [SetUp]
        public void SetUp()
        {
            _applicationSettings = new ApplicationSettings();
            _dataActions = new DataActions(_applicationSettings);
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_dataActions, Is.Not.Null);
        }

        //[Test]
        //public void GetAllCurrentMonitorJobsForThisServerHasItsParametersSetUpCorrectly()
        //{
        //    Assert.DoesNotThrow(() => _dataActions.GetAllCurrentMonitorJobsForThisServer("machineName"));
        //}

        [TearDown]
        public void TearDown()
        {

        }
    }
}