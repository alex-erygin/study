using System;
using StackExchange.Redis;

namespace Subscriber
{
    class Program
    {
        private static ConnectionMultiplexer _redis;
        private const string ChannelName = "haba";

        static void Main(string[] args)
        {
            ConnectToRedis();

            Subscribe();

            Console.ReadKey();
        }

        private static void Subscribe()
        {
            _redis.GetSubscriber()
                .Subscribe(ChannelName, (channel, value) => { Console.WriteLine("Msg received: " + value.ToString()); });
        }

        private static void ConnectToRedis()
        {
            var config = new ConfigurationOptions
            {
                AllowAdmin = true,
                EndPoints = {{"localhost", 6379}},
                AbortOnConnectFail = false,
                SyncTimeout = (int) TimeSpan.FromSeconds(5).TotalMilliseconds
            };
            _redis = ConnectionMultiplexer.Connect(config);
        }
    }
}