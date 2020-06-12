using System.Threading.Tasks;

using CfpExchange.Common;
using CfpExchange.Common.Helpers;
using CfpExchange.Common.Models;
using CfpExchange.Common.Services.Interfaces;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CfpExchange.Functions
{
    public class TweetNewCfp
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

        [FunctionName(nameof(TweetNewCfp))]
        public async Task Run(
            [ServiceBusTrigger(Constants.QUEUE_TWITTER, Connection = "ServicebusQueueConnectionString")]
            SendTweetMessage sendTweetMessage, ILogger log)
        {
            var currentEnvironment = SettingsHelper.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            if (currentEnvironment != null && !currentEnvironment.Equals("Development"))
            {
                log.LogInformation("Sending tweet...");

                await _twitterService.SendTweetAsync(sendTweetMessage);
            }
            else 
            {
                log.LogInformation("No tweet sent since we're running in Development.");
            }
        }
    }
}
