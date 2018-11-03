using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CfpExchange.Data;
using CfpExchange.Models;
using CfpExchange.Services;
using CfpExchange.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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
                var maxViews = _cfpContext.Cfps.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                    .Where(cfp => cfp.DuplicateOfId == null).Max(cfp => cfp.Views);
                indexViewModel.MostViewedCfp = _cfpContext.Cfps
                    .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                    .Where(cfp => cfp.DuplicateOfId == null).FirstOrDefault(cfp => cfp.Views == maxViews);

                // Set latest Cfp
                indexViewModel.NewestCfp = _cfpContext.Cfps
                    .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                    .Where(cfp => cfp.DuplicateOfId == null).OrderByDescending(cfp => cfp.CfpAdded).FirstOrDefault();

                // Set random
                indexViewModel.RandomCfp = _cfpContext.Cfps.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                    .Where(cfp => cfp.DuplicateOfId == null).OrderBy(o => Guid.NewGuid()).Take(1).SingleOrDefault();

                indexViewModel.CfpList = _cfpContext.Cfps
                    .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                    .Where(cfp => cfp.DuplicateOfId == null)
                    .OrderBy(cfp => cfp.CfpEndDate).Take(9).ToArray();
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
