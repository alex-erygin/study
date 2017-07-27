using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace publisher
{
    class Program
    {
        private const string QueueName = "haba-haba";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "192.168.0.10", UserName = "odmen", Password = "123456789"};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(QueueName, false, false, false, null);
                    var msg = Encoding.UTF8.GetBytes("Превед!");

                    for (;;)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        channel.BasicPublish(string.Empty, QueueName, null, msg);
                        Console.WriteLine("Сообщение отправлено");
                    }
                }
            }
        }
    }
}