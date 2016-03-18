using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;

namespace huliobot
{
    /// <summary>
    /// Бот по имени Хулио.
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            var token = SettingsStore.Tokens["hulio-token"];
            var hulio = new Api(token);
            var me = await hulio.GetMe();
            Console.WriteLine($"{me.Username} на связи");
        }
    }

    public class SettingsStore
    {
        public static readonly Dictionary<string, string> Tokens = new Dictionary<string, string>();

        static SettingsStore()
        {
            var settings = XDocument.Parse(File.ReadAllText(@"SecretSettings.xml"));
            foreach (var setting in settings.Root.Elements())
            {
                Tokens[setting.Attribute("key").Value] = setting.Attribute("value").Value;
            }
        }
        
        
    }
}
