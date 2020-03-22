using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CrystalQuartz.AspNetCore;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Quartz;
using Quartz.Impl;
using TysonFury.Jobs.Common;

namespace TysonFury
{
    public class Startup
    {
        private static IScheduler _quartzScheduler = null;

        private const string ConnectionStringName = "tyson-fury-conn-string";
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);

            var assembly = Assembly.GetExecutingAssembly();
            services.AddFluentMigratorCore()
                .ConfigureRunner(x => x.AddPostgres()
                    .WithGlobalConnectionString(ConnectionStringName)
                    .ScanIn(assembly).For.Migrations()
                    .ScanIn(assembly).For.EmbeddedResources());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureAspNet(app);

            ApplyMigrations(app);
            
            ConfigureQuartzScheduler(app);

            ConfigureMetricServer(app);
        }

        private static void ConfigureMetricServer(IApplicationBuilder app)
        {
            app.UseMetricServer();
        }

        private void ConfigureQuartzScheduler(IApplicationBuilder app)
        {
            InitializeScheduler();
            RegisterJobs().Wait();
            app.UseCrystalQuartz(() => _quartzScheduler);
            _quartzScheduler.Start().GetAwaiter().GetResult();
        }

        private static void ApplyMigrations(IApplicationBuilder app)
        {
            var helper = new MigrationHelper();
            var scope = app.ApplicationServices.CreateScope();
            var connectionString = scope.ServiceProvider.GetService<IConfiguration>().GetConnectionString(ConnectionStringName);
            var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
            var logger = scope.ServiceProvider.GetService<ILogger<MigrationHelper>>();
            helper.ApplyDatabaseMigrations(connectionString, runner, logger);
        }

        
        private static void ConfigureAspNet(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        
        private async Task RegisterJobs()
        {
            var jobDescriptors = GetType().Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(JobDescriptor)))
                .Select(x => (JobDescriptor) Activator.CreateInstance(x))
                .ToList();

            foreach (var descriptor in jobDescriptors)
            {
                if (_quartzScheduler.GetJobDetail(descriptor.Job.Key).Result == null)
                {
                    await _quartzScheduler.ScheduleJob(descriptor.Job, descriptor.Trigger);
                }
            }
        }

        private static void InitializeScheduler()
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
                    "Server=127.0.0.1;Port=5432;Database=scheduler;Userid=postgres;Password=5SNoXwnidwawj8ZdYzj9;Pooling=true;MinPoolSize=1;MaxPoolSize=2;",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz",
                ["quartz.dataSource.default.provider"] = "Npgsql"
            };

            var factory = new StdSchedulerFactory(properties);
            _quartzScheduler = factory.GetScheduler().GetAwaiter().GetResult();
        }
    }
}
