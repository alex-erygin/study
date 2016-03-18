using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using SafeConfig;
using Telegram.Bot;
using Telegram.Bot.Types;

// ReSharper disable All

namespace huliobot
{
    /// <summary>
    ///     Бот по имени Хулио.
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            HulioBot bot = new HulioBot();
            bot.Run().Wait();
        }
    }

    public class HulioBot
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Action<Api, Update>> commandHandlers =
            new Dictionary<string, Action<Api, Update>>();

        private ConfigManager configManager;
        private int offset;

        public HulioBot()
        {
            commandHandlers[Commands.Statistics] = OnFm;

            configManager = new ConfigManager().WithCurrentUserScope().Load();

            offset = configManager.Get<int>(nameof(offset));
        }

        private async void OnFm(Api api, Update update)
        {
            //get rep on stackoverflow

            HttpClient client = new HttpClient();
            string page = await client.GetStringAsync(@"http://stackoverflow.com/users/1549113/alex-erygin");
            Regex regex = new Regex(@"title=""reputation"">\s+(?<rep>\d{1,6})\s+<span");
            MatchCollection matches = regex.Matches(page);
            Match match = matches.Cast<Match>().Where(x => x.Success).FirstOrDefault();
            if (match != null)
            {
                await api.SendTextMessage(update.Message.Chat.Id, $"stack: {match.Groups["rep"].Value}");
            }
        }

        public async Task Run()
        {
            try
            {
                string token = SettingsStore.Tokens["hulio-token"];
                Api bot = new Api(token);
                User me = await bot.GetMe();
                Logger.Debug($"{me.Username} на связи");

                int offset = configManager.Load().Get<int>("offset");
                while (true)
                {
                    Update[] updates = await bot.GetUpdates(offset);

                    foreach (Update update in updates)
                    {
                        switch (update.Message.Type)
                        {
                            case MessageType.TextMessage:
                            {
                                if (commandHandlers.ContainsKey(update.Message.Text))
                                {
                                    commandHandlers[update.Message.Text](bot, update);
                                }
                            }
                                break;
                        }

                        offset = update.Id + 1;
                    }
                    configManager.Set(nameof(offset), offset).Save();
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        private class Commands
        {
            /// <summary>
            ///     Выводит статистику.
            /// </summary>
            public static string Statistics => "/fm";
        }
    }
}