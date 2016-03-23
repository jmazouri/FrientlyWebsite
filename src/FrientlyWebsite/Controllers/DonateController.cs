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
    public class DonateController : FrientlyController
    {
        public DonateController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {
            
        }

        // GET: /<controller>/
        [Authorize]
        public ActionResult Index()
        {
            //StripePlanService stripePlanService = new StripePlanService(_configuration.Get<string>("StripeApiKey"));
            //return View(stripePlanService.List().OrderByDescending(d=>d.Amount));
            return View();
        }
    }
}
