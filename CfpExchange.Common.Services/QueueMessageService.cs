using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

using CfpExchange.Common.Messages;
using CfpExchange.Common.Models;
using CfpExchange.Common.Services.Interfaces;
using CfpExchange.Common.Helpers;

namespace CfpExchange.Common.Services
{
    public class QueueMessageService : IQueueMessageService
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly IQueueClient _queueClient;

        #endregion

        #region Constructors

        public QueueMessageService(IConfiguration configuration, IQueueClient queueClient)
        {
            _queueClient = queueClient;
            _configuration = configuration;
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
            await _queueClient.SendAsync(message);
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

            await _queueClient.SendAsync(message);
        }
    }
}
