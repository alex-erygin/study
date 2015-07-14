using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;

namespace Server.MessageHandlers
{
    public class NukeAllMessageHandler : IPost<NukeAllMessage>
    {
        public object Post(NukeAllMessage request)
        {
            throw new System.NotImplementedException();
        }
    }
}