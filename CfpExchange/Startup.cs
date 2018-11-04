using System;
using System.Globalization;
using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Middleware;
using CfpExchange.Models;
using CfpExchange.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CfpExchange
{
	public class Startup
	{
		private readonly IHostingEnvironment _environment;
		private readonly ILogger _logger;

		public IConfiguration Configuration { get; }

		public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			_environment = env;
			_logger = loggerFactory.CreateLogger<Startup>();

			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables()
				.AddUserSecrets<Startup>()
				.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            if (_environment.IsDevelopment())
                services.AddDbContext<CfpContext>(opt => opt.UseInMemoryDatabase("Cfps"));
            else
                services.AddDbContext<CfpContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CfpExchangeDb")));

			services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
				.AddEntityFrameworkStores<CfpContext>()
				.AddDefaultTokenProviders();
			
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

			services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.Strict;
				options.Cookie.Expiration = TimeSpan.FromMinutes(30);
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Errors/AccessDenied";
			});

			services.AddAuthorization();

			services.AddMvc();

			AddServices(services);

		}

	    private void AddServices(IServiceCollection services)
	    {
	        if (_environment.IsProduction())
	            services.AddTransient<IEmailSender, MailGunEmailSender>();
	        else
	            services.AddTransient<IEmailSender, MockEmailSender>();

	        services.AddTransient<IDownloadEventImageMessageSender, DownloadEventImageMessageSender>();
	        services.AddTransient<ITwitterService, TwitterService>();
	        services.AddTransient<ICfpService, CfpService>();
	    }

	    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			try
			{
				using (var serviceScope = app.ApplicationServices
					.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					serviceScope.ServiceProvider.GetService<CfpContext>().Database.Migrate();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to migrate or seed database");
			}

			if (env.IsDevelopment())
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

			if (env.IsDevelopment())
			{
				using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
					serviceScope.ServiceProvider.GetService<CfpContext>().EnsureSeeded();
			}
		}
	}
}
