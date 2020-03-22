using System;
using System.Collections.Specialized;
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
using Npgsql;
using Quartz;
using Quartz.Impl;
using TysonFury.Jobs;

namespace TysonFury
{
    public class Startup
    {
        private IServiceCollection _serviceCollection = new ServiceCollection();

        private const string ConnectionStringName = "tyson-fury-conn-string";
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _serviceCollection = services;
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
        }

        private void ConfigureQuartzScheduler(IApplicationBuilder app)
        {
            var scheduler = RegisterScheduler(_serviceCollection);
            RegisterJobs(scheduler).Wait();
            app.UseCrystalQuartz(() => scheduler);
        }

        private static void ApplyMigrations(IApplicationBuilder app)
        {
            MigrationHelper.ApplyDatabaseMigrations(app.ApplicationServices, ConnectionStringName);
        }

        private static void ConfigureAspNet(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
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


        private static IScheduler RegisterScheduler(IServiceCollection services)
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
            var scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);
            services.AddHostedService<QuartzHostedService>();

            return scheduler;
        }
    }
    
    public static class MigrationHelper
    {
        /// <summary>
        /// Применяет миграции БД.pel
        /// </summary>
        public static void ApplyDatabaseMigrations(IServiceProvider serviceProvider, string connectionName)
        {
            using var scope = serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            ILogger logger = null;//scope.ServiceProvider.GetRequiredService<ILogger>();
            var connectionString = configuration.GetConnectionString(connectionName);
            try
            {
                if (!CanOpenConnection(connectionString))
                    CreateDatabase(connectionString, logger);
                TryOpenConnection(connectionString);
                runner.MigrateUp();
            }
            catch (Exception exception)
            {
                logger?.LogError(exception, "Ошибка при применении миграций БД");
                throw;
            }
        }

        private static void CreateDatabase(string connectionString, ILogger logger)
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = connectionString };

            var databaseName = builder.Database;
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                var secured = SecureConnectionString(connectionString);
                var message = $"Missing database name in connection string (ConnectionString={secured})";
                throw new InvalidOperationException(message);
            }

            builder.Database = "postgres";
            var systemDatabaseConnectionString = builder.ConnectionString;

            using (var connection = new NpgsqlConnection(systemDatabaseConnectionString))
            {
                connection.Open();

                if (DatabaseExists(databaseName, connection, logger))
                {
                    return;
                }

                var sql = $"CREATE DATABASE \"{databaseName}\" ENCODING = DEFAULT CONNECTION LIMIT = -1";
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    logger?.LogInformation($"Execute SQL command: {sql}");
                    command.ExecuteScalar();
                }
            }
        }

        private static string SecureConnectionString(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = connectionString };
            if (!string.IsNullOrWhiteSpace(builder.Password))
                builder.Password = "*****";
            return builder.ConnectionString;
        }

        private static bool CanOpenConnection(string connectionString)
        {
            try
            {
                TryOpenConnection(connectionString);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void TryOpenConnection(string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
            }
        }

        private static bool DatabaseExists(string databaseName, NpgsqlConnection connection, ILogger logger)
        {
            const string sql = "SELECT datname FROM pg_catalog.pg_database WHERE datname = @databaseName";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("databaseName", databaseName);

                logger?.LogInformation($"Execute SQL command: {sql} with parameter databaseName={databaseName}");
                var result = command.ExecuteScalar();
                logger?.LogInformation($"Database {databaseName} exists={result != null}");

                return result != null;
            }
        }
    }

}
