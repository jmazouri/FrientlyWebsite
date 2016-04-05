﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;
using FrientlyWebsite.App.Discord;
using FrientlyWebsite.Database;
using FrientlyWebsite.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FrientlyWebsite.Controllers
{
    public class EventController : FrientlyController
    {
        public EventController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {

        }

        public async Task<ActionResult> Index()
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                List<Event> result = await container.GetEvents();

                foreach (var curEvent in result)
                {
                    curEvent.UserData = await new SteamUserData(curEvent.CreatorId).Load(_configuration);
                }

                return View(result);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> NewEvent(string name, DateTime datestart, DateTime dateend)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());

                if (!await container.IsAdmin(userId)) return HttpUnauthorized();

                await container.AddEvent(new Event { Name = name, DateStart =  datestart, DateEnd = dateend, CreatorId = userId });

                DiscordIntegration.Current.SendAnnouncement($"New event from {datestart.ToString("d")} to {dateend.ToString("d")}: {name} http://friently.jmazouri.com/Event");

                return RedirectToAction("Index");
            }
        }
    }
}