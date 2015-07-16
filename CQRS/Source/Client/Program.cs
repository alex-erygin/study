using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Contracts.Messages;
using Nelibur.ServiceModel.Clients;
using NLog;

namespace Client
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var client = new SoapServiceClient("NeliburSoapService");

            Logger.Debug("Выбираем цель");

            var targets = client.Get<List<TargetToDestroy>>(new GetAllTargetsMessage());
            Logger.Debug("Получены цели: {0}", string.Join(",", targets.Select(x=>x.Name)));

            Logger.Info("Нажмите на красную кнопку для нанесения удара");
            Console.ReadKey();
            
            Logger.Info("Получено разрешение на уничтожение.");
            client.Post(new NukeAllMessage());
            Logger.Fatal("Все цели уничтожены.");
            Console.ReadKey();
        }
    }
}