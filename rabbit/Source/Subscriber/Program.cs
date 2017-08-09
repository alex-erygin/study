using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Subscriber
{
    class Program
    {
        private const string QueueName = "haba-haba";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "192.168.0.10", UserName = "odmen", Password = "123456789" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Subscription subscription = new Subscription(channel, QueueName);
                    foreach (var eventArgs in subscription)
                    {
                        Console.WriteLine("Пришло сообщение:" + Encoding.UTF8.GetString(((BasicDeliverEventArgs)eventArgs).Body));
                    }
                }
            }
        }
    }
}