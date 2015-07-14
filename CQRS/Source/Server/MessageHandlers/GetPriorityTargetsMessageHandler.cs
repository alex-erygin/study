using System.Linq;
using Contracts;
using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;

namespace Server.MessageHandlers
{
    public class GetPriorityTargetsMessageHandler : IGet<GetPriorityTargetsMessage>
    {
        public object Get(GetPriorityTargetsMessage request)
        {
            return Enumerable.Range(0, 3)
                .Select(x => new TargetToDestroy {Name = "Target #" + x}).ToList();
        }
    }
}