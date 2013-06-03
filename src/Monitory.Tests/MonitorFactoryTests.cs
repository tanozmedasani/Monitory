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
