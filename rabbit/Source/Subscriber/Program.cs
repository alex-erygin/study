using System;
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
            var factory = new ConnectionFactory() { HostName = "localhost" };
            
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Subscription subscription = new Subscription(channel, QueueName);
                    foreach (var eventArgs in subscription)
                    {
                        var senderId = new Guid(((BasicDeliverEventArgs)eventArgs).Body);
                        Console.WriteLine("Пришло сообщение от {0}", senderId);
                    }
                }
            }
        }
    }
}