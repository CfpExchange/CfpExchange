using System.Text;
using System.Threading.Tasks;
using CfpExchange.Models;
using LinqToTwitter;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CfpExchange.Functions
{
    public class TweetNewCfp : BaseFunction
    {
        [FunctionName(nameof(DownloadEventImage))]
        public async Task Run(
            [ServiceBusTrigger(Constants.QUEUE_TWITTER, Connection = "ServicebusQueueConnectionString")]
            SendTweetMessage sendTweetMessage, ILogger log)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = GetEnvironmentVariable("TwitterConsumerKey"),
                    ConsumerSecret = GetEnvironmentVariable("TwitterConsumerSecret"),
                    OAuthToken = GetEnvironmentVariable("TwitterOAuthToken"),
                    OAuthTokenSecret = GetEnvironmentVariable("TwitterOAuthTokenSecret")
                }
            };

            await auth.AuthorizeAsync();
            var ctx = new TwitterContext(auth);
            var tweetMessageBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(sendTweetMessage.TwitterHandle))
            {
                var twitterHandle = sendTweetMessage.TwitterHandle;
                if (!twitterHandle.StartsWith('@'))
                {
                    twitterHandle = "@" + twitterHandle;
                }
                tweetMessageBuilder.AppendLine($"\U0001F4E2 New CFP: {sendTweetMessage.EventName} ({twitterHandle}) ");
            }
            else
            {
                tweetMessageBuilder.AppendLine($"\U0001F4E2 New CFP: {sendTweetMessage.EventName}");
            }

            tweetMessageBuilder.AppendLine($"\U000023F3 Closes: {sendTweetMessage.CfpEndDate.ToLongDateString()}");
            if (sendTweetMessage.EventStartDate != default && sendTweetMessage.EventStartDate.Date == sendTweetMessage.EventEndDate.Date)
            {
                tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {sendTweetMessage.EventStartDate:MMM dd}");
            }
            else if (sendTweetMessage.EventStartDate != default)
            {
                tweetMessageBuilder.AppendLine($"\U0001F5D3 Event: {sendTweetMessage.EventStartDate:MMM dd} - {sendTweetMessage.EventEndDate:MMM dd}");
            }

            tweetMessageBuilder.AppendLine($"#cfp #cfpexchange {sendTweetMessage.UrlToCfp}");
            var tweetMessage = tweetMessageBuilder.ToString();
            log.LogInformation(tweetMessage);
            if (!GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT").Equals("Development"))
            {
                // TODO substringing is not the best thing, but does the trick for now
                await ctx.TweetAsync(tweetMessage.Length > 280 ? tweetMessage.Substring(0, 280) : tweetMessage,
                    sendTweetMessage.EventLocationLatitude, sendTweetMessage.EventLocationLongitude, true);
            }
        }
    }
}
