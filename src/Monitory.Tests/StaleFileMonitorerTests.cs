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