using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrientlyWebsite.App;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace FrientlyWebsite.Controllers
{
    public class AuthController : FrientlyController
    {
        public AuthController(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {

        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("~/signin")]
        public IActionResult SignIn([FromForm] string provider = "Steam")
        {
            if (Request.Query.Keys.Any(d=>d == "ReturnUrl"))
            {
                
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return new ChallengeResult(provider, new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
