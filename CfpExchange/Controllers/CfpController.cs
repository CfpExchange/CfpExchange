using System;
using System.Linq;
using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Models;
using CfpExchange.ViewModels;
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

		[HttpPost]
		public IActionResult GetMetadata(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return NotFound();

			var validatedUrl = url.ToLower();

			if (!validatedUrl.StartsWith("http://", StringComparison.Ordinal)
			    || !validatedUrl.StartsWith("https://", StringComparison.Ordinal))
				validatedUrl = $"http://{validatedUrl}";
			
			var metadata = MetaScraper.GetMetaDataFromUrl(validatedUrl);

			if (!metadata.HasData)
				return NotFound();

			return Json(metadata);
		}

		public IActionResult Browse()
		{
			return View();
		}

		public IActionResult Submit()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Submit(SubmittedCfp submittedCfp)
		{
			// TODO
			// Check validity
			if (ModelState.IsValid)
			{
				// Map
				var cfpToAddId = Guid.NewGuid();

				var cfpToAdd = new Cfp
				{
					Id = cfpToAddId,
					EventName = submittedCfp.EventTitle,
					EventUrl = submittedCfp.EventUrl,
					EventImage = submittedCfp.EventImageUrl,
					EventDescription = submittedCfp.EventDescription,
					EventLocationName = submittedCfp.LocationName,
					EventLocationLat = submittedCfp.LocationLat,
					EventLocationLng = submittedCfp.LocationLng,
					CfpEndDate = submittedCfp.CfpEndDate,
					CfpAdded = DateTime.Now
				};

				// Save CFP
				_cfpContext.Add(cfpToAdd);
				_cfpContext.SaveChanges();

				// Post to Twitter

				// Send back ID to do whatever at the client-side
				return Json(cfpToAddId);
			}

			// Add invalid model
			return BadRequest();
		}

		public IActionResult Details(Guid id)
		{
			if (id == Guid.Empty)
				return RedirectToAction("index", "home");
			
			var selectedCfp = _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Id == id);

			if (selectedCfp == null)
				// TODO to error page?
				return RedirectToAction("index", "home");

			selectedCfp.Views++;
			_cfpContext.SaveChanges();
			 
			return View(selectedCfp);
		}
	}
}