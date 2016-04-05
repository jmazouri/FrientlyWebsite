using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FrientlyWebsite.App
{
    public enum PersonaState
    {
        Offline = 0,
        Online = 1
    }

    public class SteamUserData
    {
        private static Dictionary<string, SteamUserData> _cache = new Dictionary<string, SteamUserData>(); 

        private string _steamid = null;

        private DateTime loadTime;

        [JsonProperty("avatarfull")]
        public string AvatarUrlLarge { get; private set; }

        [JsonProperty("avatar")]
        public string AvatarUrlSmall { get; private set; }

        [JsonProperty("personaname")]
        public string PersonaName { get; private set; }

        [JsonProperty("profileurl")]
        public string ProfileUrl { get; private set; }

        [JsonProperty("personastate")]
        public PersonaState PersonaState { get; private set; }

        public SteamUserData(string steamid)
        {
            _steamid = steamid;
        }

        private SteamUserData() { }

        public async Task<SteamUserData> Load(IConfiguration configuration)
        {
            if (_cache.ContainsKey(_steamid))
            {
                if (_cache[_steamid].loadTime - DateTime.UtcNow < new TimeSpan(0, 6, 0))
                {
                    return _cache[_steamid];
                }
            }

            HttpClient client = new HttpClient();
            try
            {
                string apikey = configuration.Get<string>("SteamApiKey");

                string response = await client
                    .GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apikey}&steamids={_steamid}");

                var responseObject = JObject.Parse(response)["response"]["players"][0];
                var ret = JsonConvert.DeserializeObject<SteamUserData>(responseObject.ToString());
                
                ret.loadTime = DateTime.UtcNow;
                _cache.Add(_steamid, ret);

                return _cache[_steamid];
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
