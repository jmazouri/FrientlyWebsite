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
    public class BlogController : FrientlyController
    {
        public BlogController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {
        }

        // GET: /<controller>/
        public async Task<ActionResult> Index()
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                List<BlogPost> result = await container.GetPosts();

                foreach (var post in result)
                {
                    post.UserData = await new SteamUserData(post.UserId).Load(_configuration);
                }

                bool isAdmin = false;

                if (User != null && User.Claims.Any())
                {
                    string userId = Util.GetSteamId(User.Claims.First());
                    isAdmin = await container.IsAdmin(userId);
                }

                return View(new BlogPostList { BlogPosts = result.OrderByDescending(d => d.Posted).ToList(), IsAdmin = isAdmin});
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> DeletePost(int id)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());
                if (await container.IsAdmin(userId))
                {
                    await container.DeletePost(id);
                    return RedirectToAction("Index");
                }

                return HttpUnauthorized();
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> NewPost()
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());
                if (await container.IsAdmin(userId))
                {
                    return View();
                }

                return HttpUnauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> NewPost(string title, string content, string game)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                string userId = Util.GetSteamId(User.Claims.First());

                if (!await container.IsAdmin(userId)) return HttpUnauthorized();

                await container.AddPost(new BlogPost {Content = content, Title = title, Game = game}, userId);

                if (String.IsNullOrWhiteSpace(game))
                {
                    DiscordIntegration.Current.SendAnnouncement($"New blog post: {title} http://friently.jmazouri.com/Blog");
                }
                else
                {
                    DiscordIntegration.Current.SendAnnouncement($"New blog post about {game}: {title} http://friently.jmazouri.com/Blog");
                }

                return RedirectToAction("Index");
            }
        }
    }
}
