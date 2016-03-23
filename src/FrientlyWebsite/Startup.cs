using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenId;
using AspNet.Security.OpenId.Steam;
using FrientlyWebsite.App;
using FrientlyWebsite.App.Discord;
using FrientlyWebsite.Database;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace FrientlyWebsite
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddMvc();
            services.AddAuthentication();

            services.AddInstance<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Task.Run(() => DiscordIntegration.Init(Configuration.Get<string>("DiscordUsername"), Configuration.Get<string>("DiscordPassword")));

            loggerFactory.AddDebug();
            
            loggerFactory.AddConsole(LogLevel.Verbose);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                LoginPath = new PathString("/signin")
            });

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseSteamAuthentication(new SteamAuthenticationOptions
            {
                AppKey = Configuration.Get<string>("SteamApiKey"),
                
                Events = new OpenIdAuthenticationEvents
                {
                    OnAuthenticated = async delegate(OpenIdAuthenticatedContext context)
                    {
                        using (DatabaseContainer container = new DatabaseContainer(new Logger<DatabaseContainer>(loggerFactory), Configuration))
                        {
                            await container.AddOrUpdateUser(Util.GetSteamId(context.Identifier));
                        }
                    }
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            WebApplication.Run<Startup>(args);
        } 
    }
}
