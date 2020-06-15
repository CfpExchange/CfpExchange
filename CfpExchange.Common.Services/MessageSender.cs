using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

using CfpExchange.Common.Messages;
using CfpExchange.Common.Models;
using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class MessageSender : IMessageSender
    {
        private readonly IConfiguration _configuration;
        

        public MessageSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendDownloadEventImageMessageAsync(Guid cfpPublicId, string cfpEventImageUrl)
        {
            var servicebusConnectionstring = _configuration["ServicebusEventImagesQueueConnectionString"];

            var queueClient = new QueueClient(
                new ServiceBusConnectionStringBuilder(servicebusConnectionstring),
                ReceiveMode.ReceiveAndDelete);
            var downloadEventImageModel = new DownloadEventImageMessage
            {
                Id = cfpPublicId,
                ImageUrl = cfpEventImageUrl
            };
            var messageBody = JsonConvert.SerializeObject(downloadEventImageModel);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);
        }

        public async Task SendTwitterMessageAsync(CfpInformation cfpInfo, string urlToCfp)
        {
            var servicebusConnectionstring = _configuration["ServicebusTwitterQueueConnectionString"];

            var queueClient = new QueueClient(new ServiceBusConnectionStringBuilder(servicebusConnectionstring));
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

            await queueClient.SendAsync(message);
        }
    }
}
