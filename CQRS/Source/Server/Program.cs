using Contracts.Messages;
using Nelibur.ServiceModel.Services;
using Server.MessageHandlers;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureNelibur();
        }

        private static void ConfigureNelibur()
        {
            NeliburSoapService.Configure(x =>
            {
                x.Bind<DropTheBombMessage, DropTheBombMessageHandler>();
                x.Bind<GetAllTargetsMessage, GetAllTargetsMessagesHandler>();
                x.Bind<GetPriorityTargetsMessage, GetPriorityTargetsMessageHandler>();
                x.Bind<NukeAllMessage, NukeAllMessageHandler>();
            });
        }
    }
}