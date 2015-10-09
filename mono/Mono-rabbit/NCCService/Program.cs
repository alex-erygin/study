using System;
using RabbitMQ.Client;
using System.Threading;

namespace NCCService
{
	class MainClass
	{
		private const string QueueName = "haba-haba";

		public static void Main (string[] args)
		{
			var publisherId = Guid.NewGuid();
			Console.WriteLine("Я - отправитель {0}", publisherId);

			var factory = new ConnectionFactory { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(QueueName, false, false, false, null);
					var msg = publisherId.ToByteArray();

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