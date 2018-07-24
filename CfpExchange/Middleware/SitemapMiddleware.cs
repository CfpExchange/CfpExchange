using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CfpExchange.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CfpExchange.Middleware
{
	public class SitemapMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _rootUrl;
		private readonly CfpContext _cfpContext;

		public SitemapMiddleware(RequestDelegate next, string rootUrl, CfpContext cfpContext)
		{
			_next = next;
			_rootUrl = rootUrl;
			_cfpContext = cfpContext;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Path.Value.Equals("/sitemap.xml", StringComparison.OrdinalIgnoreCase))
			{
				var stream = context.Response.Body;
				context.Response.StatusCode = 200;
				context.Response.ContentType = "application/xml";

				string sitemapContent = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";

				var controllers = Assembly.GetExecutingAssembly().GetTypes()
					.Where(type => typeof(Controller).IsAssignableFrom(type)
						|| type.Name.EndsWith("controller", StringComparison.Ordinal)).ToList();

				foreach (var controller in controllers)
				{
					var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Where(method => typeof(IActionResult).IsAssignableFrom(method.ReturnType));

					foreach (var method in methods)
					{
						sitemapContent += "<url>";
						sitemapContent += $"<loc>{_rootUrl}/{controller.Name.ToLower().Replace("controller", "")}/{method.Name.ToLower()}</loc>";
						sitemapContent += string.Format("<lastmod>{0}</lastmod>", DateTime.UtcNow.ToString("yyyy-MM-dd"));
						sitemapContent += "</url>";
					}
				}

				foreach (var cfp in _cfpContext.Cfps.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow))
				{
					sitemapContent += "<url>";
					sitemapContent += $"<loc>{_rootUrl}/cfp/details/{cfp.Slug}</loc>";
					sitemapContent += $"<lastmod>{cfp.CfpAdded.ToString("yyyy-MM-dd")}</lastmod>";
					sitemapContent += "</url>";
				}

				sitemapContent += "</urlset>";

				using (var memoryStream = new MemoryStream())
				{
					var bytes = Encoding.UTF8.GetBytes(sitemapContent);
					memoryStream.Write(bytes, 0, bytes.Length);
					memoryStream.Seek(0, SeekOrigin.Begin);
					await memoryStream.CopyToAsync(stream, bytes.Length);
				}
			}
			else
			{
				await _next(context);
			}
		}
	}

	public static partial class BuilderExtensions
	{
		public static IApplicationBuilder UseRssMiddleware(this IApplicationBuilder app,
			string rootUrl = "http://localhost:5000")
		{
			var serviceScope = app.ApplicationServices
				.GetRequiredService<IServiceScopeFactory>().CreateScope();

			return app.UseMiddleware<RssMiddleware>(rootUrl, serviceScope.ServiceProvider.GetService<CfpContext>());
		}
	}
}