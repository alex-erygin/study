using System;
using Contracts.Messages;
using Nelibur.ServiceModel.Services.Operations;
using Server.Commands;

namespace Server.MessageHandlers
{
    public class NukeAllMessageHandler : IPostOneWay<NukeAllMessage>
    {
        private readonly ICommandService _commandService;

        public NukeAllMessageHandler(ICommandService commandService)
        {
            if (commandService == null) throw new ArgumentNullException("commandService");
            _commandService = commandService;
        }

        public void PostOneWay(NukeAllMessage request)
        {
            _commandService.Execute(new NukeAllCommand());
        }
    }
}