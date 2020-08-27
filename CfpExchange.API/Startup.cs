using System.Collections.Generic;

using CfpExchange.API.Models;
using CfpExchange.Common.Data;
using CfpExchange.Common.Models;
using CfpExchange.Common.Services;
using CfpExchange.Common.Services.Interfaces;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Nelibur.ObjectMapper;

namespace CfpExchange.API
{
    public class Startup
    {
        #region Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        public void ConfigureServices(IServiceCollection services)
        {
            RegisterMappings();
            services.AddControllers();
#if DEBUG
            services.AddDbContext<CfpContext>(opt => opt.UseInMemoryDatabase("Cfps"));
            services.AddTransient<IEmailService, MockEmailService>();
#else
                services.AddDbContext<CfpContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CfpExchangeDb")));
                services.AddTransient<IEmailService, MailgunEmailService>();
#endif
            services.AddTransient<ICfpService, CfpService>();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CFP.Exchange API");
                c.RoutePrefix = string.Empty;
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
#if DEBUG
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetService<CfpContext>().EnsureSeeded();
#endif
        }

        private void RegisterMappings()
        {
            TinyMapper.Bind<Cfp, CfpData>();
            TinyMapper.Bind<CfpData, Cfp>();
            TinyMapper.Bind<List<Cfp>, List<CfpData>>();
        }
    }
}
