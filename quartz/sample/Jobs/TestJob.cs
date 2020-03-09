using System;
using System.Threading.Tasks;
using Quartz;

namespace sample.Jobs
{
    public class TestJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Все в дыму... Не видно кода...");
            return Task.CompletedTask;
        }
    }
}