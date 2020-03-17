using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace TysonFury
{
    public class QuartzHostedService: IHostedService
    {
        private readonly IScheduler _scheduler;


        public QuartzHostedService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _scheduler.Start(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _scheduler.Shutdown(cancellationToken);
        }
    }
}
