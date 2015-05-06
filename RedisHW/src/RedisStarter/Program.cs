using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using StackExchange.Redis;

namespace Publisher
{
    class Program
    {
        private const string ChannelName = "haba";
        private static ConnectionMultiplexer _redis;

        static void Main(string[] args)
        {
            StartRedis();
            
            ConnectToRedis();

            SendMessages();

            Console.ReadLine();
        }

        private static void SendMessages()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("send msg to subscribers");
                var msg = "haba" + i;
                _redis.GetSubscriber().Publish(ChannelName, msg, CommandFlags.FireAndForget);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
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


        private static void StartRedis()
        {
            const int OneHundredMegaByte = 100 * 1024 * 1024;
            string redisDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "redis");
            string configFile = Path.Combine(redisDirectory, "redis.windows.conf");
            string arguments = string.Format("{0} --maxheap {1}", configFile, OneHundredMegaByte);
            var processInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(redisDirectory, "redis-server.exe"),
                Arguments = arguments,
                UseShellExecute = true
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();
        }
    }
}