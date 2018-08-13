using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CfpExchange.Data;
using CfpExchange.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CfpExchange.Middleware
{
	public class RssMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _rootUrl;
		private readonly CfpContext _cfpContext;

		public RssMiddleware(RequestDelegate next, string rootUrl, CfpContext cfpContext)
		{
			_next = next;
			_rootUrl = rootUrl;
			_cfpContext = cfpContext;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Path.Value.Equals("/rss/newest", StringComparison.OrdinalIgnoreCase))
			{
				var stream = context.Response.Body;
				context.Response.StatusCode = 200;
				context.Response.ContentType = "application/rss+xml";

				string rssContent = "<?xml version=\"1.0\"?>" +
					"<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\">" +
									"<channel>" +
									$"<atom:link href=\"{Constants.WebsiteRootUrl}rss/newest\" rel=\"self\" type=\"application/rss+xml\" />" +
									"<title>CFP Exchange - New CFPs</title>" +
									$"<link>{Constants.WebsiteRootUrl}</link>" +
									"<description>This feed nofities you of the newest CFPs in our system.</description>";

				foreach (var cfp in _cfpContext.Cfps.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow))
				{
					rssContent += "<item>";
					rssContent += $"<title>{cfp.EventName}, CFP closes: {cfp.CfpEndDate.ToString("dd MMM yyyy")}</title>";
					rssContent += $"<pubDate>{cfp.CfpAdded.ToString("r")}</pubDate>";
					rssContent += $"<link>{_rootUrl}/cfp/details/{cfp.Slug}</link>";
					rssContent += $"<guid>{_rootUrl}/cfp/details/{cfp.Slug}</guid>";
					rssContent += $"<description>{cfp.EventDescription}</description>";
					rssContent += "</item>";
				}

				rssContent += "</channel>" +
					"</rss>";

				using (var memoryStream = new MemoryStream())
				{
					var bytes = Encoding.UTF8.GetBytes(rssContent);
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
		public static IApplicationBuilder UseSitemapMiddleware(this IApplicationBuilder app,
			string rootUrl = "http://localhost:5000")
		{
			var serviceScope = app.ApplicationServices
				.GetRequiredService<IServiceScopeFactory>().CreateScope();

			return app.UseMiddleware<SitemapMiddleware>(rootUrl, serviceScope.ServiceProvider.GetService<CfpContext>());

		}
	}
}