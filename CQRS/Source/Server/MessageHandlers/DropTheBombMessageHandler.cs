using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;

namespace Server.MessageHandlers
{
    public class DropTheBombMessageHandler : IPost<DropTheBombMessage>
    {
        public object Post(DropTheBombMessage request)
        {
            throw new System.NotImplementedException();
        }
    }
}