using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;
using FrientlyWebsite.App.Discord;
using FrientlyWebsite.Database;
using FrientlyWebsite.Models;
using FrientlyWebsite.Models.ViewModels;
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
                    foreach (var eventCommit in curEvent.Commitments)
                    {
                        eventCommit.UserData = await new SteamUserData(eventCommit.UserId).Load(_configuration);
                    }
                }

                string userId = Util.GetSteamId(User.Claims.FirstOrDefault());
                bool isAdmin = await container.IsAdmin(userId);

                return View(new EventList { Events = result , IsAdmin = isAdmin });
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> DeleteEvent(int id)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());
                if (await container.IsAdmin(userId))
                {
                    await container.DeleteEvent(id);
                    return RedirectToAction("Index");
                }

                return HttpUnauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> NewEvent(string name, DateTime date, DateTime timestart, DateTime timeend)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());

                if (!await container.IsAdmin(userId)) return HttpUnauthorized();

                DateTime datestart = date.AddTicks(timestart.TimeOfDay.Ticks);
                DateTime dateend = date.AddTicks(timeend.TimeOfDay.Ticks);

                await container.AddEvent(new Event { Name = name, DateStart =  datestart, DateEnd = dateend, CreatorId = userId });

                if (dateend == DateTime.MinValue)
                {
                    DiscordIntegration.Current.SendAnnouncement(
                        $"New event on {datestart.ToString("d")}: {name} http://friently.jmazouri.com/Event");
                }
                else
                {
                    DiscordIntegration.Current.SendAnnouncement(
                        $"New event from {datestart.ToString("h:mm tt")} to {dateend.ToString("h:mm tt")}: {name} http://friently.jmazouri.com/Event");
                }

                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateEventCommitment(int id, CommitmentState newState, string comment = null)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());

                await container.AddOrUpdateCommitment(id, userId, newState, comment);

                return Ok();
            }
        }
    }
}
