using System;
using System.Linq;
using System.Threading.Tasks;
using CfpExchange.Data;
using CfpExchange.Helpers;
using CfpExchange.Models;
using CfpExchange.Services;
using CfpExchange.ViewModels;
using LinqToTwitter;
using Microsoft.AspNetCore.Hosting;
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
		private readonly IEmailSender _emailSender;
		private readonly IHostingEnvironment _hostingEnvironment;

		public CfpController(CfpContext cfpContext, IConfiguration configuration,
			IEmailSender emailSender, IHostingEnvironment env)
		{
			_cfpContext = cfpContext;
			_configuration = configuration;
			_emailSender = emailSender;
			_hostingEnvironment = env;
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

			// Double check image URL
			metadata.ImageUrl = ValidateImageUri(metadata.ImageUrl);

			return Json(metadata);
		}

		[HttpPost]
		public IActionResult CheckDuplicates(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return NotFound();

			var validatedUrl = url.ToLower();

			if (!validatedUrl.StartsWith("http://", StringComparison.Ordinal)
				&& !validatedUrl.StartsWith("https://", StringComparison.Ordinal))
				validatedUrl = $"http://{validatedUrl}";

			Uri.TryCreate(validatedUrl, UriKind.Absolute, out var parsedUri);

			var alreadyInDatabase = _cfpContext.Cfps
				.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
				.Where(c => c.EventUrl.ToLowerInvariant().Contains(parsedUri.Host) || c.CfpUrl.ToLowerInvariant().Contains(parsedUri.Host));

			var result = new
			{
				IsKnown = alreadyInDatabase.Any(),
				SimilarCfps = alreadyInDatabase.ToArray()
			};

			return Json(result);
		}

		[HttpGet]
		public IActionResult Browse(int page = 1, string searchTerm = "")
		{
			int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

			var lowercaseSearchTerm = searchTerm?.ToLowerInvariant() ?? "";

			var allActiveCfps = _cfpContext.Cfps
				.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
				.Where(cfp => cfp.EventName.ToLowerInvariant().Contains(lowercaseSearchTerm)
					|| cfp.EventLocationName.ToLowerInvariant().Contains(lowercaseSearchTerm))
				.OrderBy(cfp => cfp.CfpEndDate)
				.Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
				.Take(MaximumNumberOfItemsPerPage)
				.ToList();

			return View(new BrowseResponseViewModel(allActiveCfps, pageToShow, searchTerm));
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
					EventImage = ValidateImageUri(submittedCfp.EventImageUrl),
					EventDescription = submittedCfp.EventDescription,
					EventLocationName = submittedCfp.LocationName,
					EventLocationLat = submittedCfp.LocationLat,
					EventLocationLng = submittedCfp.LocationLng,
					CfpEndDate = submittedCfp.CfpEndDate.Date,
					CfpAdded = DateTime.Now,
					CfpUrl = submittedCfp.CfpUrl,
					EventStartDate = submittedCfp.EventStartDate?.Date ?? default(DateTime),
					EventEndDate = submittedCfp.EventEndDate?.Date ?? default(DateTime),
					ProvidesAccommodation = submittedCfp.ProvidesAccommodation,
					ProvidesTravelAssistance = submittedCfp.ProvidesTravelAssistance,
					SubmittedByName = submittedCfp.SubmittedByName
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

					if (_hostingEnvironment.IsProduction())
					{
						// TODO substringing is not the best thing, but does the trick for now
						await ctx.TweetAsync(tweetMessage.Length > 280 ? tweetMessage.Substring(0, 280) : tweetMessage,
							(decimal)cfpToAdd.EventLocationLat, (decimal)cfpToAdd.EventLocationLng);
					}
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

		public async Task<IActionResult> SendReportIssue(Issue issue)
		{
			if (ModelState.IsValid)
			{
				var bodyText = $"An issue was reported for CFP: https://cfp.exchange/cfp/details/{issue.CfpId}"
					+ Environment.NewLine + $"Issue: {issue.Description}";

				await _emailSender.SendEmailAsync(_configuration["AdminEmailaddress"],
					$"{issue.Name} <{issue.EmailAddress}>", "Issue with CFP", bodyText);

				return Ok();
			}

			// TODO: return invalid model
			return BadRequest();
		}

		[HttpGet]
		public IActionResult OutgoingCfpLink(Guid id, string url)
		{
			try
			{
				var linkedCfp = _cfpContext.Cfps.Single(cfp => cfp.Id == id);
				linkedCfp.ClicksToCfpUrl++;

				var foo = _cfpContext.SaveChanges();
			}
			catch
			{
				// Intentionally left blank, should be a show-stopper
			}

			return Redirect(url);
		}

		private string ValidateImageUri(string eventImageUrl)
		{
			if (eventImageUrl == Constants.NoEventImageUrl)
				return eventImageUrl;
			
			string imageUri = Constants.NoEventImageUrl;

			if (Uri.TryCreate(eventImageUrl, UriKind.Absolute, out var parsedUri))
				imageUri = parsedUri.ToString();

			return imageUri;
		}
	}
}