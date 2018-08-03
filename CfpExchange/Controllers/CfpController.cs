using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;

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

			// Inject api key here to prevent resolve of a service for this one value in the underlying class
			//var metadata = MetaScraper.GetUrlPreview(validatedUrl, _configuration["UrlPreviewApiKey"]);
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
		public IActionResult Browse(int page = 1, string eventdate = "0", string searchTerm = "")
		{
			int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

			var lowercaseSearchTerm = searchTerm?.ToLowerInvariant() ?? "";

			var startDateTime = DateTime.UtcNow;
			var endDateTime = DateTime.UtcNow.AddYears(10);

			if (eventdate != "0")
			{
				if (DateTime.TryParseExact(eventdate, "yyyy-M", Culture.US, System.Globalization.DateTimeStyles.None, out startDateTime))
				{
					endDateTime = startDateTime.AddMonths(1).AddDays(-1);
				}
			}

			var allActiveCfps = _cfpContext.Cfps
				.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
				.Where(cfp => cfp.DuplicateOfId == null)
				.Where(cfp => cfp.EventName.ToLowerInvariant().Contains(lowercaseSearchTerm)
					|| cfp.EventLocationName.ToLowerInvariant().Contains(lowercaseSearchTerm)
					|| cfp.EventTags.ToLowerInvariant().Contains(lowercaseSearchTerm))
				.Where(cfp => cfp.EventStartDate == default(DateTime) || cfp.EventEndDate == default(DateTime) || cfp.EventStartDate >= startDateTime && cfp.EventEndDate <= endDateTime)
				.OrderBy(cfp => cfp.CfpEndDate)
				.Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
				.Take(MaximumNumberOfItemsPerPage)
				.ToList();

			return View(new BrowseResponseViewModel(allActiveCfps, pageToShow, searchTerm, eventdate));
		}

		[HttpGet]
		public IActionResult Newest(int page = 1)
		{
			int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

			var allActiveCfps = _cfpContext.Cfps
				.Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
				.Where(cfp => cfp.DuplicateOfId == null)
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
		public async Task<IActionResult> Submit([FromForm]SubmittedCfp submittedCfp)
		{
			// TODO
			// Check validity
			if (ModelState.IsValid)
			{
				// Map
				var cfpToAddId = Guid.NewGuid();
				var timezone = string.Empty;

				try
				{
					timezone = await GetTimezone(submittedCfp.LocationLat, submittedCfp.LocationLng);
				}
				catch
				{
					// Intentionally left blank, just for event to calendar
					// If it fails, sucks for you
				}

				var cfpToAddSlug = FriendlyUrlHelper.GetFriendlyTitle(submittedCfp.EventTitle);
				var i = 0;

				// Prevent duplicate slugs
				while (_cfpContext.Cfps.Any(cfp => cfp.Slug == cfpToAddSlug))
				{
					cfpToAddSlug = $"{cfpToAddSlug}-{++i}";
				}

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
					SubmittedByName = submittedCfp.SubmittedByName,
					EventTwitterHandle = submittedCfp.EventTwitterHandle,
					EventTimezone = timezone,
					Slug = cfpToAddSlug,
					EventTags = submittedCfp.EventTags
				};

				// Save CFP
				_cfpContext.Add(cfpToAdd);
				_cfpContext.SaveChanges();

				// Post to Twitter account
				try
				{
					await PostNewCfpTweet(cfpToAdd);
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
			return BadRequest(submittedCfp);
		}

		private async Task PostNewCfpTweet(Cfp cfpToAdd)
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

			var tweetMessageBuilder = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(cfpToAdd.EventTwitterHandle))
			{
				var twitterHandle = cfpToAdd.EventTwitterHandle;

				if (!twitterHandle.StartsWith('@'))
					twitterHandle = "@" + twitterHandle;

				tweetMessageBuilder.AppendLine($"\U0001F4E2 New CFP: {cfpToAdd.EventName} ({twitterHandle}) ");
			}
			else
				tweetMessageBuilder.AppendLine($"\U0001F4E2 New CFP: {cfpToAdd.EventName}");

			tweetMessageBuilder.AppendLine($"\U000023F3 Closes: {cfpToAdd.CfpEndDate.ToLongDateString()}");

			if (cfpToAdd.EventStartDate != default(DateTime) && cfpToAdd.EventStartDate.Date == cfpToAdd.EventEndDate.Date)
				tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {cfpToAdd.EventStartDate.ToString("MMM dd")}");
			else if (cfpToAdd.EventStartDate != default(DateTime))
				tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {cfpToAdd.EventStartDate.ToString("MMM dd")} - {cfpToAdd.EventEndDate.ToString("MMM dd")}");

			tweetMessageBuilder.AppendLine($"#cfp #cfpexchange {Url.Action("details", "cfp", new { id = cfpToAdd.Id }, "https", "cfp.exchange")}");

			var tweetMessage = tweetMessageBuilder.ToString();

			if (_hostingEnvironment.IsProduction())
			{
				// TODO substringing is not the best thing, but does the trick for now
				await ctx.TweetAsync(tweetMessage.Length > 280 ? tweetMessage.Substring(0, 280) : tweetMessage,
					(decimal)cfpToAdd.EventLocationLat, (decimal)cfpToAdd.EventLocationLng, true);
			}
			else
			{
				Debug.WriteLine(tweetMessage);
			}
		}

		private async Task<string> GetTimezone(double lat, double lng)
		{
			// Only in production, saves credits
			if (_hostingEnvironment.IsProduction())
			{
				using (var httpClient = new HttpClient())
				{
					var resultJson = await httpClient.GetStringAsync($"https://atlas.microsoft.com/timezone/byCoordinates/json?subscription-key={_configuration["MapsApiKey"]}&api-version=1.0&query={lat}%2C{lng}");
					var result = JsonConvert.DeserializeObject<TimezoneInfo>(resultJson);

					return result.TimeZones.FirstOrDefault()?.Id ?? string.Empty;
				}
			}

			return string.Empty;
		}

		public IActionResult Details(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				return RedirectToAction("index", "home");

			var selectedCfp = _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Slug == id);

			if (selectedCfp == null)
			{
				// Check it the id happens to be a Guid
				if (Guid.TryParse(id, out Guid guidId))
				{
					if (guidId != Guid.Empty)
						selectedCfp = _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Id == guidId);
				}

				if (selectedCfp == null)
					// TODO to error page?
					return RedirectToAction("index", "home");
			}

			if (selectedCfp.DuplicateOfId != null && selectedCfp.DuplicateOfId != Guid.Empty)
			{
				var originalCfp = _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Id == selectedCfp.DuplicateOfId);
				return RedirectToAction("details", "cfp", new { id = originalCfp.Slug });
			}

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
				// Intentionally left blank, shouldn't be a show-stopper
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