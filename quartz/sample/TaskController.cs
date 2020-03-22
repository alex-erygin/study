using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;
using TysonFury.Jobs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TysonFury
{
    [Route("api/task")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;

        public TaskController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }

        // GET: api/<controller>
        [HttpGet]
        public Task Get()
        {
            _logger.LogError("Coronavirus detected!");
            return Task.CompletedTask;
        }
    }
}
