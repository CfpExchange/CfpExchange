using System;
using System.Linq;
using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Models;
using Microsoft.AspNetCore.Mvc;

namespace CfpExchange.Controllers
{
	public class CfpController : Controller
	{
		private readonly CfpContext _cfpContext;

		public CfpController(CfpContext cfpContext)
		{
			_cfpContext = cfpContext;
		}

		public IActionResult Submit()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Submit(Cfp submittedCfp)
		{
			// Check validity

			// Save CFP

			// Post to Twitter

			return View();
		}

		public IActionResult Details(Guid id)
		{
			if (id == Guid.Empty)
				RedirectToAction("index", "home");
			
			var selectedCfp = _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Id == id);

			// Error handling
			 
			return View(selectedCfp);
		}
	}
}