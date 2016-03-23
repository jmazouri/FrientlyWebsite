using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;

namespace FrientlyWebsite.App.Discord
{
    public class DiscordIntegration
    {
        private static DiscordIntegration _currentIntegration;
        public static DiscordIntegration Current => _currentIntegration;

        private DiscordClient _client;

        public static async Task Init(string username, string password)
        {
            /*
            if (!Util.IsUnix())
            {
                return;
            }
            */

            var ret = new DiscordIntegration
            {
                _client = new DiscordClient(new DiscordConfigBuilder
                {
                    AppName = "Friently Website"
                })
            };

            await ret._client.Connect(username, password);

            _currentIntegration = ret;
        }

        private DiscordIntegration()
        {
            
        }

        public async void SendAnnouncement(string message)
        {
            if (Util.IsUnix())
            {
                await _client.Servers.First().FindChannels("announcements").First().SendMessage("@everyone " + message);
            }
            else
            {
                await _client.Servers.First().FindChannels("bot_tests").First().SendMessage("@everyone " + message);
            }
        }
    }
}
