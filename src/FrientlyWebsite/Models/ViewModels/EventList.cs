using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrientlyWebsite.Models.ViewModels
{
    public class EventList
    {
        public List<Event> Events { get; set; }
        public bool IsAdmin { get; set; }
    }
}
