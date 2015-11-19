using System;
using System.Security.Claims;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Localization;
using Microsoft.Extensions.PlatformAbstractions;
using System.Globalization;

using Team.Models;


namespace Team
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            IsDevelopment = env.IsDevelopment();
        }

        public IConfigurationRoot Configuration { get; set; }
        
        public bool IsDevelopment { get; set; }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(config);

            // Setup options with DI
            services.AddOptions();
            // Add logging to DI
            services.AddLogging();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policyBuilder =>
                {
                    policyBuilder.RequireClaim(
                        ClaimTypes.Name,
                        Configuration["AppSettings:AdminUsers"].Split(',')
                    );
                });
            });

            string connectionString = Configuration["Data:DefaultConnection:ConnectionString"];

            // pull from connection string in azure/appsettings until I get a fix for this...
            if (!IsDevelopment)
            {
                connectionString = Configuration["ConnectionString"];
            }

            services.AddEntityFramework()
              .AddSqlServer()
              .AddDbContext<ApplicationDbContext>(options =>
              {
                  options.UseSqlServer(connectionString);
              });
            
            // Add MVC services to the services container.
            services.AddMvc(options =>
            {
                if (!IsDevelopment)
                {
                   options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            // register services...    
            services.AddScoped<ISurventrixService, SurventrixService>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            // set locale to GB
            app.UseRequestLocalization(new RequestCulture(new CultureInfo("en-GB")));
            
            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();
            
            app.UseCookieAuthentication(options =>
            {
                options.AutomaticAuthenticate = true;
                options.LoginPath = "/";
                if (env.IsProduction())
                {
                    options.CookieSecure = CookieSecureOption.Always;
                }
            });

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });


            // allow pinging and ponging... :)
            app.Use((context, next) =>
            {
                Console.WriteLine("{0} {1}{2}{3}",
                     context.Request.Method,
                     context.Request.PathBase,
                     context.Request.Path,
                     context.Request.QueryString);

                if (context.Request.Path.StartsWithSegments("/ping"))
                {
                    return context.Response.WriteAsync("pong");
                }
                return next();
            });
        }
    }
}
