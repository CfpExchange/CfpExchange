using System;
using CfpExchange.Data;
using CfpExchange.Models;
using CfpExchange.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CfpExchange
{
	public class Startup
	{
		private IHostingEnvironment _environment;
   
		public Startup(IHostingEnvironment env)
		{
			_environment = env;

			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables()
				.AddUserSecrets("CfpExchangeSecrets")
				.Build();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			if (_environment.IsDevelopment())
				services.AddDbContext<CfpContext>(opt => opt.UseInMemoryDatabase("Cfps"));
			else
				services.AddDbContext<CfpContext>(opt => opt.UseSqlServer(Configuration["CfpExchangeDb"]));

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

            // After signing up at MailGun (https://www.mailgun.com/) swap the following two lines:
			// services.AddTransient<IEmailSender, MailGunEmailSender>();
            services.AddTransient<IEmailSender, MockEmailSender>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

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