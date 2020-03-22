using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using TysonFury;
using Xunit;

namespace Tests
{
    public class JobFinderTests
    {
        [Fact]
        public void FindJobMigrations_AssemblyWithJobMigrations_ListWithJobs()
        {
            var finder = new JobFinder(GetType().Assembly);
            var jobs = finder.FindJobMigrations();
            Assert.NotEmpty(jobs);
        }

        [Fact]
        public void FindJobMigrations_AssemblyWithMigrations_NotEmptyMigrationVersion()
        {
            var finder = new JobFinder(GetType().Assembly);
            var job = finder.FindJobMigrations().FirstOrDefault(x=> x.GetType() == typeof(TestJobMigration));
            var version = (JobVersionAttribute)Attribute.GetCustomAttribute(job.GetType(), typeof(JobVersionAttribute));
            Assert.Equal("COVID-19", version.Version);
            Assert.NotNull(job.ToInstall);
            Assert.NotNull(job.ToRemove);
        }
    }

    [JobVersion(version:"COVID-19")]
    public class TestJobMigration : JobMigration
    {
        public TestJobMigration()
        {
            var trigger = TriggerBuilder.Create().Build();
            ToInstall = new List<JobInfo> {new JobInfo(new FightCovidJob(), trigger)};
            ToRemove = new List<JobInfo> {new JobInfo(new FightCovidJob(), trigger)};
        }
        
        public override List<JobInfo> ToInstall { get; }
        
        public override List<JobInfo> ToRemove { get; }
    }

    public class FightCovidJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }
}