using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using TysonFury.Jobs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TysonFury
{
    [Route("api/task")]
    public class TaskController : Controller
    {
        private IScheduler _scheduler;

        public TaskController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }
    }
}
