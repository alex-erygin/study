using System;
using System.Threading.Tasks;
using Quartz;
using TysonFury.Jobs.Common;

namespace TysonFury.Jobs
{
    public class TestJobDescriptor : JobDescriptor
    {
        public TestJobDescriptor()
        {
            Job = JobBuilder
                .CreateForAsync<TestJob>()
                .WithIdentity("Smoke Test")
                .WithDescription("snoop dog")
                .Build();
            
            Trigger = TriggerBuilder
                .Create()
                .WithIdentity("every-day-trigger")
                .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(10).RepeatForever().Build())
                .StartNow()
                .Build();
        }
        
        
        public override ITrigger Trigger { get; }
        
        
        public override IJobDetail Job { get; }


        private class TestJob : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                Console.WriteLine("Все в дыму... Не видно кода... Трям тарарам.");
                return Task.CompletedTask;
            }
        }
    }
}