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
            _badFilesFolderMonitorer.Process(new MonitorJob { MontiredJobType = MontiredJobType.BadFileDirectory, Path = @"..\monitorey.Integration.Tests\TestBadFileFolder" });

            _emailActions.AssertWasCalled(x => x.SendAlert("There are 'Bad Files' in the directory ..\\monitorey.Integration.Tests\\TestBadFileFolder"));
        }


    }
}
