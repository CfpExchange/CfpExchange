using System;
using System.Linq;
using System.Threading.Tasks;
using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Models;
using CfpExchange.ViewModels;
using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CfpExchange.Controllers
{
	public class CfpController : Controller
	{
	    private const int MaximumPageToShow = 3000;
	    private const int MaximumNumberOfItemsPerPage = 10;

        private readonly CfpContext _cfpContext;
		private readonly IConfiguration _configuration;

		public CfpController(CfpContext cfpContext, IConfiguration configuration)
		{
			_cfpContext = cfpContext;
			_configuration = configuration;
		}

		[HttpPost]
		public IActionResult GetMetadata(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return NotFound();

			var validatedUrl = url.ToLower();

			if (!validatedUrl.StartsWith("http://", StringComparison.Ordinal)
				&& !validatedUrl.StartsWith("https://", StringComparison.Ordinal))
				validatedUrl = $"http://{validatedUrl}";
			
			var metadata = MetaScraper.GetMetaDataFromUrl(validatedUrl);

			if (!metadata.HasData)
				return NotFound();

			return Json(metadata);
		}

        [HttpGet]
        public IActionResult Browse(int page = 1)
        {
            int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

            var allActiveCfps = _cfpContext.Cfps
		        .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
		        .OrderBy(cfp => cfp.CfpEndDate)
                .Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
		        .Take(MaximumNumberOfItemsPerPage)
		        .ToList();

	        return View(new BrowseResponseViewModel(allActiveCfps, pageToShow));
        }

	    [HttpGet]
	    public IActionResult Newest(int page = 1)
	    {
	        int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

            var allActiveCfps = _cfpContext.Cfps
	            .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                .OrderByDescending(cfp => cfp.CfpAdded.Date)
                .ThenBy(cfp => cfp.CfpEndDate.Date)
	            .Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
	            .Take(MaximumNumberOfItemsPerPage)
	            .ToList();

	        return View(new NewestResponseViewModel(allActiveCfps, pageToShow));
	    }

        [HttpGet]
		public IActionResult Submit()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Submit(SubmittedCfp submittedCfp)
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

				// Post to Twitter account
				try
				{
					var auth = new SingleUserAuthorizer
					{
						CredentialStore = new SingleUserInMemoryCredentialStore
						{
							ConsumerKey = _configuration["TwitterConsumerKey"],
							ConsumerSecret = _configuration["TwitterConsumerSecret"],
							OAuthToken = _configuration["TwitterOAuthToken"],
							OAuthTokenSecret = _configuration["TwitterOAuthTokenSecret"]
						}
					};

					await auth.AuthorizeAsync();

					var ctx = new TwitterContext(auth);

					var tweetMessage = $"New CFP Added: {cfpToAdd.EventName} closes {cfpToAdd.CfpEndDate.ToLongDateString()} #cfpexchange {cfpToAdd.EventUrl}";

					// TODO substringing is not the best thing, but does the trick for now
					await ctx.TweetAsync(tweetMessage.Length > 280 ? tweetMessage.Substring(0, 280) : tweetMessage,
						(decimal)cfpToAdd.EventLocationLat, (decimal)cfpToAdd.EventLocationLng);
				}
				catch
				{
					// Intentionally left blank, we can probably do something
					// more useful, but for now if Twitter fails  ¯\_(ツ)_/¯
				}

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