using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FrientlyWebsite.App
{
    public class FrientlyController : Controller
    {
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly IConfiguration _configuration;
        private ILoggerFactory loggerFactory;

        public FrientlyController(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }
    }
}
