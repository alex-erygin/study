using NLog;
using Server.Commands;

namespace Server.CommandHandlers
{
    public class DropTheBombCommandHandler : ICommandHandler<DropTheBombCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Handle(DropTheBombCommand command)
        {
            Logger.Debug("Boooooom! from DropTheBombCommand");
        }
    }
}