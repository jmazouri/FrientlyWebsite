using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;
using FrientlyWebsite.Database;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FrientlyWebsite.Controllers
{
    public class HomeController : FrientlyController
    {
        public HomeController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {
        }

        // GET: /<controller>/
        public ActionResult Index()
        {
            return View();
        }
    }
}
