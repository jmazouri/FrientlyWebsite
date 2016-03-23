using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;

namespace FrientlyWebsite.Models
{
    public class BlogPost
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Posted { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Game { get; set; }
        public SteamUserData UserData { get; set; }
    }
}
