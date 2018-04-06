using System;
using Microsoft.AspNetCore.Mvc;

namespace CfpExchange.Controllers
{
	public class CfpController : Controller
	{
		public IActionResult Submit()
		{
			return View();
		}

		public IActionResult Details(Guid id)
		{
			return View();
		}
	}
}