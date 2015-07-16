using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;
using Server.Commands;

namespace Server.MessageHandlers
{
    public class DropTheBombMessageHandler : IPost<DropTheBombMessage>
    {
        private readonly ICommandService _commandService;

        public DropTheBombMessageHandler(ICommandService commandService)
        {
            _commandService = commandService;
        }

        public object Post(DropTheBombMessage message)
        {
            _commandService.Execute(new DropTheBombCommand(message.Target));
        }
    }
}