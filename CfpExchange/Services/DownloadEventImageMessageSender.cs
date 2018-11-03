using System;
using System.Text;
using System.Threading.Tasks;
using CfpExchange.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CfpExchange.Services
{
    public class DownloadEventImageMessageSender : IDownloadEventImageMessageSender
    {
        private readonly IConfiguration _configuration;
        

        public DownloadEventImageMessageSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Execute(Guid cfpPublicId, string cfpEventImageUrl)
        {
            var servicebusConnectionstring = _configuration["ServicebusEventImagesQueueConnectionString"];

            var queueClient = new QueueClient(
                new ServiceBusConnectionStringBuilder(servicebusConnectionstring),
                ReceiveMode.ReceiveAndDelete);
            var downloadEventImageModel = new DownloadEventImage
            {
                Id = cfpPublicId,
                ImageUrl = cfpEventImageUrl
            };
            var messageBody = JsonConvert.SerializeObject(downloadEventImageModel);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await queueClient.SendAsync(message);
        }
    }
}
