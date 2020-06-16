using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;

using CfpExchange.Common.Messages;
using CfpExchange.Common.Models;
using CfpExchange.Common.Services.Interfaces;
using CfpExchange.Common.Helpers;

namespace CfpExchange.Common.Services
{
    public class QueueMessageService : IQueueMessageService
    {
        #region Fields

        private readonly ITwitterQueueClient _twitterQueueClient;
        private readonly IDownloadImageQueueClient _downloadImageQueueClient;

        #endregion

        #region Constructors

        public QueueMessageService(ITwitterQueueClient twitterQueueClient, IDownloadImageQueueClient downloadImageQueueClient)
        {
            _twitterQueueClient = twitterQueueClient;
            _downloadImageQueueClient = downloadImageQueueClient;
        }

        #endregion

        public async Task SendDownloadEventImageMessageAsync(Guid cfpPublicId, string cfpEventImageUrl)
        {
            Guard.IsNotNull(cfpEventImageUrl, nameof(cfpEventImageUrl));

            var downloadEventImageModel = new DownloadEventImageMessage
            {
                Id = cfpPublicId,
                ImageUrl = cfpEventImageUrl
            };
            var messageBody = JsonConvert.SerializeObject(downloadEventImageModel);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await _downloadImageQueueClient.SendAsync(message);
        }

        public async Task SendTwitterMessageAsync(CfpInformation cfpInfo, string urlToCfp)
        {
            Guard.IsNotNull(cfpInfo, nameof(cfpInfo));
            Guard.IsNotNull(urlToCfp, nameof(urlToCfp));

            var sendTweetMessage = new SendTweetMessage
            {
                CfpEndDate = cfpInfo.CfpEndDate,
                EventStartDate = cfpInfo.EventStartDate,
                EventEndDate = cfpInfo.EventEndDate,
                EventLocationLatitude = cfpInfo.EventLocationLatitude,
                EventLocationLongitude = cfpInfo.EventLocationLongitude,
                EventName = cfpInfo.EventName,
                TwitterHandle = cfpInfo.TwitterHandle,
                UrlToCfp = urlToCfp
            };
            var messageBody = JsonConvert.SerializeObject(sendTweetMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await _twitterQueueClient.SendAsync(message);
        }
    }
}
