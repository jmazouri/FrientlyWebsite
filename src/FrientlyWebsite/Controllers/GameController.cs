using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;
using FrientlyWebsite.Database;
using FrientlyWebsite.Models.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FrientlyWebsite.Controllers
{
    public class GameController : FrientlyController
    {
        private static string TekkitBackupsFolder = @"E:\aaas";
        private static string FactorioBackupsFolder = @"E:\aaas";

        public GameController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {
            if (Util.IsUnix())
            {
                TekkitBackupsFolder = "/var/tekkitserver/ForgeEssentials/Backups/world/DIM_0";
                FactorioBackupsFolder = "/var/factorio/saves";
            }
        }

        private async Task<BlogPostList> GetBlogPosts(string gameid)
        {
            using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(_loggerFactory), _configuration))
            {
                var posts = (await container.GetPosts()).Where(d => d.Game == gameid);

                foreach (var post in posts)
                {
                    post.UserData = await new SteamUserData(post.UserId).Load(_configuration);
                }

                bool isAdmin = false;

                if (User != null && User.Claims.Any())
                {
                    string userId = Util.GetSteamId(User.Claims.First());
                    isAdmin = await container.IsAdmin(userId);
                }

                return new BlogPostList
                {
                    BlogPosts = posts.ToList(),
                    IsAdmin = isAdmin
                };
            }
        }

        // GET: /<controller>/
        public async Task<ActionResult> TekkitLegends()
        {
            return View(await GetBlogPosts("Tekkit"));
        }

        // GET: /<controller>/
        public async Task<ActionResult> TeamFortress2()
        {
            return View(await GetBlogPosts("TF2"));
        }

        public async Task<ActionResult> Factorio()
        {
            return View(await GetBlogPosts("Factorio"));
        }

        public ActionResult FactorioBackups()
        {
            return View(Directory.GetFiles(FactorioBackupsFolder, "*.zip").Select(d => new FileInfo(d)).OrderByDescending(d => d.CreationTime));
        }

        public ActionResult TekkitBackups()
        {
            return View(Directory.GetFiles(TekkitBackupsFolder).Select(d=>new FileInfo(d)).OrderByDescending(d=>d.CreationTime));
        }

        public ActionResult DownloadTekkitBackup(string backupname)
        {
            string backupPath = Path.Combine(TekkitBackupsFolder, Path.GetFileName(backupname));

            if (System.IO.File.Exists(backupPath))
            {
                return File(new FileStream(backupPath, FileMode.Open),
                    "application/zip", "friently-tekkit-backup-" + new FileInfo(backupPath).CreationTime.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".zip");
            }

            return HttpNotFound();
        }

        public ActionResult DownloadFactorioBackup(string backupname)
        {
            string backupPath = Path.Combine(FactorioBackupsFolder, Path.GetFileName(backupname));

            if (System.IO.File.Exists(backupPath))
            {
                return File(new FileStream(backupPath, FileMode.Open),
                    "application/zip", "friently-factorio-backup-" + new FileInfo(backupPath).CreationTime.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".zip");
            }

            return HttpNotFound();
        }
    }
}
