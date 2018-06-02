using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CfpExchange.Models;
using CfpExchange.Data;
using System.Linq;
using CfpExchange.ViewModels;
using CfpExchange.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CfpExchange.Controllers
{
	public class HomeController : Controller
	{
		private readonly CfpContext _cfpContext;
		private readonly IEmailSender _emailSender;
		private readonly IConfiguration _configuration;

		public HomeController(CfpContext cfpContext, IEmailSender emailSender,
		                      IConfiguration configuration)
		{
			_cfpContext = cfpContext;
			_emailSender = emailSender;
			_configuration = configuration;
		}

		public IActionResult Index()
		{
			var indexViewModel = new IndexViewModel();

			// Set most viewed
			if (_cfpContext.Cfps.Any())
			{
				var maxViews = _cfpContext.Cfps.Max(cfp => cfp.Views);
				indexViewModel.MostViewedCfp = _cfpContext.Cfps.FirstOrDefault(cfp => cfp.Views == maxViews);

				// Set latest Cfp
				indexViewModel.NewestCfp = _cfpContext.Cfps.OrderByDescending(cfp => cfp.CfpAdded).FirstOrDefault();

				// TODO set real random
				indexViewModel.RandomCfp = _cfpContext.Cfps.FirstOrDefault();

				// TODO set real CFP of the day
				indexViewModel.CfpOfTheDay = _cfpContext.Cfps.FirstOrDefault();

				indexViewModel.CfpList = _cfpContext.Cfps.OrderBy(cfp => cfp.CfpEndDate).Take(10).ToArray();
			}

			return View(indexViewModel);
		}

		public IActionResult About()
		{
			return View();
		}

		public IActionResult Contact(bool mailSent)
		{
			return View(mailSent);
		}

		[HttpPost]
		public async Task<IActionResult> SendContact(string name, string emailaddress, string subject, string message)
		{
			await _emailSender.SendEmailAsync(_configuration["AdminEmailaddress"], $"{name} <{emailaddress}>", subject, message);

			return RedirectToAction("Contact", "Home", new { mailSent = true });
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}