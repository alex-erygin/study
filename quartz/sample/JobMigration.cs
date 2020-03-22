using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quartz;

namespace TysonFury
{
    public class JobInstaller
    {
        private readonly IJobFinder _jobFinder;

        public JobInstaller(IJobFinder jobFinder)
        {
            _jobFinder = jobFinder ?? throw new ArgumentNullException(nameof(jobFinder));
        }

        public void Install()
        {
            
        }
    }

    public interface IJobFinder
    {
        List<JobMigration> FindJobMigrations();
    }

    public class JobFinder : IJobFinder
    {
        private readonly List<Assembly> _assembliesWithJobs;

        public JobFinder(params Assembly[] assembliesWithJobs)
        {
            if (assembliesWithJobs == null) throw new ArgumentNullException(nameof(assembliesWithJobs));
            
            _assembliesWithJobs = assembliesWithJobs.ToList();

            if (!_assembliesWithJobs.Any())
            {
                _assembliesWithJobs.Add(GetType().Assembly);
            }
        }
        
        public List<JobMigration> FindJobMigrations()
        {
            var types = _assembliesWithJobs
                .SelectMany(x =>
                    x.GetTypes().Where(t => t.GetCustomAttributes(typeof(JobVersionAttribute)).Any()))
                .ToList();

            var jobMigrations = types    
                .Select(x => (JobMigration) Activator.CreateInstance(x));

            return jobMigrations.ToList();
        }
    }
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JobVersionAttribute: Attribute
    {
        public JobVersionAttribute(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(version));
            Version = version;
        }
        
        public string Version { get; }
    }
    
    
    public abstract class JobMigration
    {
        public abstract List<JobInfo> ToInstall { get; }
        
        public abstract List<JobInfo> ToRemove { get; }
    }
    
    
    public class JobInfo
    {
        public JobInfo(IJob job, ITrigger trigger)
        {
            Job = job;
            Trigger = trigger;
        }
        
        public IJob Job { get; }
        
        public ITrigger Trigger { get; }
    }
}