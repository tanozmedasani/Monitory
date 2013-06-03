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
using monitory.BusinessObjects;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Tests
{
    [TestFixture]
    public class MonitorJobActionsTests
    {
        MonitorJobActions _monitorJobActions;
        ITimeActions _timeActions;
        IApplicationSettings _applicationSettings;

        [SetUp]
        public void SetUp()
        {
            _applicationSettings = MockRepository.GenerateStub<IApplicationSettings>();
            _timeActions = MockRepository.GenerateStub<ITimeActions>();
            _monitorJobActions = new MonitorJobActions(_timeActions, _applicationSettings);
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_monitorJobActions, Is.Not.Null);
        }

        [Test]
        public void NextTimeThisJobShouldRunReturnsTheExpectedDate()
        {
            var lastTimeRan = new DateTime(2013, 4, 24, 14, 00, 00);
            var monitorJob = new MonitorJob
                {
                    LastTimeThisJobRan = lastTimeRan,
                    Threshold = 5,
                    ThresholdType = ThresholdType.Minutes,
                };
            var result = _monitorJobActions.NextTimeThisJobShouldRun(monitorJob);
            Assert.That(result, Is.EqualTo(lastTimeRan.AddMinutes(5)));
        }

        [Test]
        public void ReturnsTrueWhenTheLastRunTimeIsNull()
        {
            var monitorJob = new MonitorJob
                {
                    LastTimeThisJobRan = null,
                };
            var result = _monitorJobActions.ThisJobShouldRunNow(monitorJob);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnsTrueWhenItsTimeToRun()
        {
            _timeActions.Stub(x => x.Now()).Return(new DateTime(2013, 4, 23));

            var monitorJob = new MonitorJob
                {
                    LastTimeThisJobRan = new DateTime(2013, 4, 22),
                    Threshold = 5,
                    ThresholdType = ThresholdType.Seconds,
                };
            var result = _monitorJobActions.ThisJobShouldRunNow(monitorJob);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnsFalseWhenItIsNotTimeToRun()
        {
            _timeActions.Stub(x => x.Now()).Return(new DateTime(2013, 4, 24));

            var monitorJob = new MonitorJob
            {
                LastTimeThisJobRan = new DateTime(2013, 4, 24),
                Threshold = 5,
                ThresholdType = ThresholdType.Seconds,
            };
            var result = _monitorJobActions.ThisJobShouldRunNow(monitorJob);

            Assert.That(result, Is.False);
        }
        
        [Test]
        public void WeAreInTheRunWindowReturnsTrueWhenItShould()
        {
            _timeActions.Stub(x => x.Now()).Return(new DateTime(2013, 4, 25, 8, 30, 1));
            _applicationSettings.Stub(x => x.HourToStartMonitoring).Return(6);
            _applicationSettings.Stub(x => x.HourToStopMonitoring).Return(17);
            var result = _monitorJobActions.WeAreInTheRunWindow();
            Assert.That(result, Is.True);
        }

        [Test]
        public void WeAreInTheRunWindowReturnsFalseWhenItShould()
        {
            _timeActions.Stub(x => x.Now()).Return(new DateTime(2013, 4, 25, 22, 30, 1));
            _applicationSettings.Stub(x => x.HourToStartMonitoring).Return(6);
            _applicationSettings.Stub(x => x.HourToStopMonitoring).Return(17);
            var result = _monitorJobActions.WeAreInTheRunWindow();
            Assert.That(result, Is.False);
        }

        [Test]
        public void JobsetHasExpiredReturnsTrueWhenTheCreateDatePlusTimeForJobSetToLiveIsLessThanNow()
        {
            MonitorJobSet monitorJobSet = new MonitorJobSet(); //create date set right now
            _timeActions.Stub(x => x.Now()).Return(DateTime.Now.AddDays(100)); //this means that the job must have expired
            _applicationSettings.Stub(x => x.MinutesBetweenCheckingForNewMonitorJobs).Return(15);
            var result = _monitorJobActions.JobsetHasExpired(monitorJobSet);
            Assert.That(result, Is.True);
        }

        [Test]
        public void JobsetHasExpiredReturnsFalseWhenTheCreateDatePlusTimeForJobSetToLiveIsMoreThanNow()
        {
            MonitorJobSet monitorJobSet = new MonitorJobSet(); //create date set right now
            _timeActions.Stub(x => x.Now()).Return(DateTime.Now.AddDays(-100)); //this means that the job cannot have expired
            _applicationSettings.Stub(x => x.MinutesBetweenCheckingForNewMonitorJobs).Return(15);
            var result = _monitorJobActions.JobsetHasExpired(monitorJobSet);
            Assert.That(result, Is.False);
        }


        [TearDown]
        public void TearDown()
        {

        }
    }
}