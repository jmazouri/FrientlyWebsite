using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;

namespace FrientlyWebsite.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Name { get; set; }
        public string CreatorId { get; set; }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public List<EventCommitment> Commitments { get; set; } 

        public SteamUserData UserData { get; set; }
    }
}
