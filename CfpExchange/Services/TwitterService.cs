using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CfpExchange.Models;
using LinqToTwitter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CfpExchange.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public TwitterService(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task PostNewCfpTweet(Cfp cfpToAdd, string urlToCfp)
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
                tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {cfpToAdd.EventStartDate:MMM dd}");
            else if (cfpToAdd.EventStartDate != default(DateTime))
                tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {cfpToAdd.EventStartDate:MMM dd} - {cfpToAdd.EventEndDate:MMM dd}");

            tweetMessageBuilder.AppendLine($"#cfp #cfpexchange {urlToCfp}");

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
    }
}
