using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using NUnit.Framework;

namespace monitory.Tests
{
    [TestFixture]
    public class TimeActionsTests
    {
        ITimeActions _timeActions;

        [SetUp]
        public void SetUp()
        {
            _timeActions = new TimeActions();
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_timeActions, Is.Not.Null);
        }

    }
}