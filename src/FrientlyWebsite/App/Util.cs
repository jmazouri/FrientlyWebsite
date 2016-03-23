using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FrientlyWebsite.App
{
    public static class Util
    {
        public static string GetSteamId(string url)
        {
            return url.Substring(url.LastIndexOf('/') + 1);
        }

        public static string GetSteamId(Claim claim)
        {
            return GetSteamId(claim.Value);
        }

        public static bool IsUnix()
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 128);
        }
    }
}
