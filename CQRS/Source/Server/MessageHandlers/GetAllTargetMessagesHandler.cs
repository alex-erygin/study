using System.Linq;
using Contracts;
using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;

namespace Server.MessageHandlers
{
    public class GetAllTargetMessageHandler : IGet<GetAllTargetsMessage>
    {
        public object Get(GetAllTargetsMessage request)
        {
            return Enumerable.Range(0, 100)
                .Select(x => new TargetToDestroy {Name = "Target #" + x}).ToList();
        }
    }
}