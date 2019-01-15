using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CfpExchange.Helpers;
using CfpExchange.Models;
using CfpExchange.Services;
using CfpExchange.ViewModels;
using LinqToTwitter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CfpExchange.Controllers
{
    public class CfpController : Controller
    {
        private const int MaximumPageToShow = 3000;

        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDownloadEventImageMessageSender _downloadEventImageMessageSender;
        private readonly ITwitterService _twitterService;
        private readonly ICfpService _cfpService;
        private ILogger<CfpController> _logger;

        public CfpController(
            IConfiguration configuration,
            IEmailSender emailSender,
            IHostingEnvironment env,
            IDownloadEventImageMessageSender downloadEventImageMessageSender,
            ITwitterService twitterService,
            ICfpService cfpService,
            ILogger<CfpController> logger)
        {
            _configuration = configuration;
            _emailSender = emailSender;
            _hostingEnvironment = env;
            _downloadEventImageMessageSender = downloadEventImageMessageSender;
            _twitterService = twitterService;
            _cfpService = cfpService;
            _logger = logger;
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

            var alreadyInDatabase = _cfpService.IsAlreadyInDatabase(parsedUri);

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

            var allActiveCfps = _cfpService.GetAllActiveCfps(lowercaseSearchTerm, startDateTime, endDateTime, pageToShow);

            return View(new BrowseResponseViewModel(allActiveCfps, pageToShow, searchTerm, eventdate));
        }

        [HttpGet]
        public IActionResult Newest(int page = 1)
        {
            int pageToShow = page <= MaximumPageToShow ? page : MaximumPageToShow;

            var allActiveCfps = _cfpService.GetNewestActiveCfps(pageToShow);

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
                var cfpToAdd = await CreateCfpToAdd(submittedCfp);

                _cfpService.AddCfp(cfpToAdd);

                await _cfpService.SaveChangesAsync();

                await DownloadEventImageLocally(submittedCfp, cfpToAdd);

                await PostNewCfpTweet(cfpToAdd);

                // Send back ID to do whatever at the client-side
                return Json(cfpToAdd.Id);
            }

            // Add invalid model
            return BadRequest(submittedCfp);
        }

        private async Task PostNewCfpTweet(Cfp cfpToAdd)
        {
            // Post to Twitter account
            try
            {
                await _twitterService.PostNewCfpTweet(cfpToAdd,
                    Url.Action("details", "cfp", new { id = cfpToAdd.Id }, "https", "cfp.exchange"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task DownloadEventImageLocally(SubmittedCfp submittedCfp, Cfp cfpToAdd)
        {
            try
            {
                if (ShouldDownloadEventImageLocally())
                {
                    await _downloadEventImageMessageSender.Execute(cfpToAdd.Id, submittedCfp.EventImageUrl);
                }
                else
                {
                    _logger.LogInformation("Not downloading event image locally.");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task<Cfp> CreateCfpToAdd(SubmittedCfp submittedCfp)
        {
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
            var cfpToAddSlug = GetCfpSlug(submittedCfp);

            var cfpToAdd = new Cfp
            {
                Id = Guid.NewGuid(),
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
                EventTags = submittedCfp.EventTags,
                CfpDecisionDate = submittedCfp.CfpDecisionDate?.Date ?? default(DateTime)
            };

            return cfpToAdd;
        }

        private string GetCfpSlug(SubmittedCfp submittedCfp)
        {
            var cfpToAddSlug = FriendlyUrlHelper.GetFriendlyTitle(submittedCfp.EventTitle);
            var i = 0;

            // Prevent duplicate slugs
            while (_cfpService.CfpWithIdenticalSlugExists(cfpToAddSlug))
            {
                cfpToAddSlug = $"{cfpToAddSlug}-{++i}";
            }

            return cfpToAddSlug;
        }

        private bool ShouldDownloadEventImageLocally()
        {
            return bool.TryParse(_configuration["FeatureToggle:HostOwnImages"], out bool hostOwnImages) && hostOwnImages;
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

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("index", "home");

            var selectedCfp = _cfpService.GetCfpBySlug(id);

            if (selectedCfp == null)
            {
                // Check it the id happens to be a Guid
                if (Guid.TryParse(id, out Guid guidId))
                {
                    if (guidId != Guid.Empty)
                        selectedCfp = _cfpService.GetCfpById(guidId);
                }

                if (selectedCfp == null)
                    // TODO to error page?
                    return RedirectToAction("index", "home");
            }

            if (selectedCfp.DuplicateOfId != null && selectedCfp.DuplicateOfId != Guid.Empty)
            {
                var originalCfp = _cfpService.GetCfpById((Guid) selectedCfp.DuplicateOfId);
                return RedirectToAction("details", "cfp", new { id = originalCfp.Slug });
            }

            selectedCfp.Views++;

            await _cfpService.SaveChangesAsync();

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
        public async Task<IActionResult> OutgoingCfpLink(Guid id, string url)
        {
            var linkedCfp = _cfpService.GetCfpById(id);

            if (linkedCfp != null)
            {
                linkedCfp.ClicksToCfpUrl++;

                await _cfpService.SaveChangesAsync();
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
