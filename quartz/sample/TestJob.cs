using System;
using System.Threading.Tasks;
using Quartz;

namespace sample
{
    public class TestJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Hello world");
            return Task.CompletedTask;
        }
    }
}