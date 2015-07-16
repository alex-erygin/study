using Contracts.Messages;
using Microsoft.Practices.Unity;
using Nelibur.ServiceModel.Services;
using Server.MessageHandlers;

namespace Server
{
    class Program
    {
        private static readonly UnityContainer Container = new UnityContainer();

        static void Main(string[] args)
        {
            new ServiceBuilder(Container).RegisterDependencies();
            ConfigureNelibur();
        }

        private static void ConfigureNelibur()
        {
            NeliburSoapService.Configure(x =>
            {
                x.Bind<DropTheBombMessage, DropTheBombMessageHandler>(() => Container.Resolve<DropTheBombMessageHandler>());
                x.Bind<GetAllTargetsMessage, GetAllTargetsMessagesHandler>(() => Container.Resolve<GetAllTargetsMessagesHandler>());
                x.Bind<GetPriorityTargetsMessage, GetPriorityTargetsMessageHandler>(() => Container.Resolve<GetPriorityTargetsMessageHandler>());
                x.Bind<NukeAllMessage, NukeAllMessageHandler>(() => Container.Resolve<NukeAllMessageHandler>());
            });
        }
    }
}