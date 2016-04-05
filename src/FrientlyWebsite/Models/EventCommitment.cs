using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;

namespace FrientlyWebsite.Models
{
    public enum CommitmentState
    {
        Confirmed,
        Rejected,
        Maybe
    }

    public class EventCommitment
    {
        public string UserId { get; set; }
        public CommitmentState CommitmentState { get; set; }
        public string Comment { get; set; }

        public SteamUserData UserData { get; set; }
    }
}
