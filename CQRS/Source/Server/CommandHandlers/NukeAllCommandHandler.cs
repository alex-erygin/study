using NLog;
using Server.Commands;

namespace Server.CommandHandlers
{
    public class NukeAllCommandHandler : ICommandHandler<NukeAllCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Handle(NukeAllCommand command)
        {
            Logger.Fatal("Exterminatus performed");
        }
    }
}