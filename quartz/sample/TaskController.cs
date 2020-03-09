using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using sample.Jobs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sample
{
    [Route("api/task")]
    public class TaskController : Controller
    {
        private IScheduler _scheduler;

        public TaskController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        // GET: api/<controller>
        [HttpGet]
        public Task Get()
        {
            var jobDetails = JobBuilder
                .CreateForAsync<TestJob>()
                .WithIdentity("Da Job")
                .WithDescription("Da big Job")
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .StartNow()
                .Build();

            return _scheduler.ScheduleJob(jobDetails, trigger);
        }
    }
}
