using System.Threading.Tasks;

using CfpExchange.Models;
using CfpExchange.Services.Interfaces;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CfpExchange.Functions
{
    public class TweetNewCfp : BaseFunction
    {
        #region Fields

        private readonly ITwitterService _twitterService;

        #endregion

        #region Constructors

        public TweetNewCfp(ITwitterService twitterService)
        {
            _twitterService = twitterService;
        }

        #endregion

        [FunctionName(nameof(DownloadEventImage))]
        public async Task Run(
            [ServiceBusTrigger(Constants.QUEUE_TWITTER, Connection = "ServicebusQueueConnectionString")]
            SendTweetMessage sendTweetMessage, ILogger log)
        {
            if (!GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT").Equals("Development"))
            {
                await _twitterService.SendTweetAsync(sendTweetMessage);
            }
            else 
            {
                log.LogInformation("No tweet sent since we're running in Development.");
            }
        }
    }
}
