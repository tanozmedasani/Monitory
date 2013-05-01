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