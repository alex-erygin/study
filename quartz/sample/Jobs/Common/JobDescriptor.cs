using Quartz;

namespace TysonFury.Jobs.Common
{
    public abstract class JobDescriptor
    {
        public abstract ITrigger Trigger { get; }
        
        public abstract IJobDetail Job { get; }
    }
}