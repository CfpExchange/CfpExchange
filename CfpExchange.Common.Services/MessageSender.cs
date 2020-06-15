using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

using CfpExchange.Common.Models;
using CfpExchange.Models;
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

        public async Task SendTwitterMessageAsync(Cfp cfpToAdd, string urlToCfp)
        {
            var servicebusConnectionstring = _configuration["ServicebusTwitterQueueConnectionString"];

            var queueClient = new QueueClient(new ServiceBusConnectionStringBuilder(servicebusConnectionstring));
            var sendTweetMessage = new SendTweetMessage
            {
                CfpEndDate = cfpToAdd.CfpEndDate,
                EventStartDate = cfpToAdd.EventStartDate,
                EventEndDate = cfpToAdd.EventEndDate,
                EventLocationLatitude = (decimal)cfpToAdd.EventLocationLat,
                EventLocationLongitude = (decimal)cfpToAdd.EventLocationLng,
                EventName = cfpToAdd.EventName,
                TwitterHandle = cfpToAdd.EventTwitterHandle,
                UrlToCfp = urlToCfp
            };
            var messageBody = JsonConvert.SerializeObject(sendTweetMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
        }
    }
}
