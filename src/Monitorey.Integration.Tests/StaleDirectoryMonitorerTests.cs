using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using monitory.BusinessObjects;
using monitory.Infrastructure;
using monitory.Infrastructure.Interfaces;
using monitory.Infrastructure.MonitorClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace monitory.Integration.Tests
{
    [TestFixture]
    public class StaleDirectoryMonitorerTests
    {
        StaleDirectoryMonitorer _staleDirectoryMonitorer;
        IEmailActions _emailActions;
        ITimeActions _timeActions;

        [SetUp]
        public void SetUp()
        {
            _timeActions = new TimeActions();
            _emailActions = MockRepository.GenerateStub<IEmailActions>();
            _staleDirectoryMonitorer = new StaleDirectoryMonitorer(_emailActions, _timeActions);
            
        }

        [Test]
        public void CanBeInstanced()
        {
            Assert.That(_staleDirectoryMonitorer, Is.Not.Null);
        }

        [Test]
        public void StaleDirectoryMonitorerCallsEmailActionsWhenThereIsAFileOlderThanTheThreshold()
        {
            var monitorJobToUse = new MonitorJob
                {
                    MontiredJobType = MontiredJobType.StaleDirectory,
                    Path = @"..\monitory.Integration.Tests\TestStaleDirectory",
                    Threshold = -15,//NOTE: Negative on purpose so other tests that touch the directory don't make this one fail
                    ThresholdType = ThresholdType.Minutes,
                };

            _staleDirectoryMonitorer.Process(monitorJobToUse);

            _emailActions.AssertWasCalled(x => x.SendAlert("No new files have shown up in the directory '..\\monitory.Integration.Tests\\TestStaleDirectory' in the last '-15' 'Minutes'"));
        }

        [Test]
        public void StaleDirectoryMonitorerDoesNotCallEmailActionsWhenThereWhenTheFolderHasBeenChangedInsideTheThreshold()
        {
            string path = @"..\monitory.Integration.Tests\TestStaleDirectory";
            string fileName = "testNewFile.txt";

            IdentityReference everybodyIdentity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            var rule = new FileSystemAccessRule(
                everybodyIdentity,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            DirectorySecurity directorySecurity = Directory.GetAccessControl(path);
            directorySecurity.AddAccessRule(rule);
            Directory.SetAccessControl(path, directorySecurity);
            
            File.WriteAllText(Path.Combine(path, fileName), "content");

            _staleDirectoryMonitorer.Process(new MonitorJob
            {
                MontiredJobType = MontiredJobType.StaleDirectory,
                Path = path,
                Threshold = 1,
                ThresholdType = ThresholdType.Minutes,
            });

            File.Delete(Path.Combine(path, fileName));

            _emailActions.AssertWasNotCalled(x => x.SendAlert("No new files have shown up in the directory '..\\monitory.Integration.Tests\\TestStaleDirectory' in the last '1' 'Minutes'"));

        }

    }
}