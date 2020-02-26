using System;
using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Middleware;
using CfpExchange.Models;
using CfpExchange.Services;

namespace CfpExchange
{
    public class Startup
    {
        #region Fields

        public IConfiguration Configuration { get; }

        #endregion

        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env.Equals("Development"))
            {
                services.AddDbContext<CfpContext>(opt => opt.UseInMemoryDatabase("Cfps"));
            }
            else
            {
                services.AddDbContext<CfpContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CfpExchangeDb")));
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<CfpContext>()
                .AddDefaultTokenProviders();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Errors/AccessDenied";
            });

            services.AddAuthorization();
            services.AddMvc((mvcOptions) => mvcOptions.EnableEndpointRouting = false);

            if (env.Equals("Development"))
            {
                services.AddTransient<IEmailSender, MockEmailSender>();
            }
            else
            {
                services.AddTransient<IEmailSender, MailGunEmailSender>();
            }
            services.AddTransient<IMessageSender, MessageSender>();
            services.AddTransient<ICfpService, CfpService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                using var serviceScope = app.ApplicationServices
                    .GetRequiredService<IServiceScopeFactory>().CreateScope();
                serviceScope.ServiceProvider.GetService<CfpContext>().Database.Migrate();
            }
            catch
            {
            }

            if (env.EnvironmentName.Equals("Development"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSitemapMiddleware();
                app.UseRssMiddleware();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseSitemapMiddleware(Constants.WebsiteRootUrl);
                app.UseRssMiddleware(Constants.WebsiteRootUrl);
            }

            var supportedCultures = new[]
            {
                new CultureInfo("en-US")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            if (env.EnvironmentName.Equals("Development"))
            {
                using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                serviceScope.ServiceProvider.GetService<CfpContext>().EnsureSeeded();
            }
        }
    }
}
