using System.Collections.Specialized;
using System.Threading.Tasks;
using CrystalQuartz.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using sample.Jobs;

namespace sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            RegisterDependencies(services);

            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            var scheduler = app.ApplicationServices.GetService<IScheduler>();
            RegisterJobs(scheduler).Wait();
            app.UseCrystalQuartz(() => scheduler);
        }


        private async Task RegisterJobs(IScheduler scheduler)
        {
            await AddSmokeTestJob(scheduler);
        }

        private static async Task AddSmokeTestJob(IScheduler scheduler)
        {
            var jobDetails = JobBuilder
                .CreateForAsync<TestJob>()
                .WithIdentity("Smoke Test")
                .WithDescription("snoop dog")
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity("every-day-trigger")
                .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(10).RepeatForever().Build())
                .StartNow()
                .Build();

            if (scheduler.GetJobDetail(jobDetails.Key).Result == null)
            {
                await scheduler.ScheduleJob(jobDetails, trigger);
            }
        }


        private static void RegisterDependencies(IServiceCollection services)
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "TikTak",
                ["quartz.scheduler.instanceId"] = "single_instance",
                ["quartz.threadPool.threadCount"] = "2",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.useProperties"] = "true",
                ["quartz.jobStore.dataSource"] = "default",
                ["quartz.serializer.type"] = "json",
                ["quartz.jobStore.tablePrefix"] = "qrtz_",
                ["quartz.dataSource.default.connectionString"] =
                    "Server=127.0.0.1;Port=5432;Database=scheduler;Userid=postgres;Password=;Pooling=true;MinPoolSize=1;MaxPoolSize=2;",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz",
                ["quartz.dataSource.default.provider"] = "Npgsql"
            };

            var factory = new StdSchedulerFactory(properties);
            var scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);
            services.AddHostedService<QuartzHostedService>();
        }
    }
}
