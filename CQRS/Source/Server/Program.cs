using System;
using System.ServiceModel;
using Contracts.Messages;
using Microsoft.Practices.Unity;
using Nelibur.ServiceModel.Services;
using Nelibur.ServiceModel.Services.Default;
using Server.MessageHandlers;

namespace Server
{
    class Program
    {
        private static readonly UnityContainer Container = new UnityContainer();
        private static ServiceHost _service;

        static void Main(string[] args)
        {
            ConfigureNelibur();
            RunService();
        }

        private static void ConfigureNelibur()
        {
            new ServiceBuilder(Container).RegisterDependencies();
            NeliburSoapService.Configure(x =>
            {
                x.Bind<DropTheBombMessage, DropTheBombMessageHandler>(() => Container.Resolve<DropTheBombMessageHandler>());
                x.Bind<GetAllTargetsMessage, GetAllTargetsMessagesHandler>(() => Container.Resolve<GetAllTargetsMessagesHandler>());
                x.Bind<GetPriorityTargetsMessage, GetPriorityTargetsMessageHandler>(() => Container.Resolve<GetPriorityTargetsMessageHandler>());
                x.Bind<NukeAllMessage, NukeAllMessageHandler>(() => Container.Resolve<NukeAllMessageHandler>());
            });
        }

        private static void RunService()
        {
            _service = new ServiceHost(typeof(SoapServicePerCall));
            _service.Open();
            Console.WriteLine("Bomber service is running");
            Console.WriteLine("Press any key to exit\n");
            Console.ReadKey();
            _service.Close();
        }
    }
}