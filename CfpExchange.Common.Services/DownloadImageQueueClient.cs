using Microsoft.Azure.ServiceBus;

using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class DownloadImageQueueClient : QueueClient, IDownloadImageQueueClient
    {
        public DownloadImageQueueClient(string connectionString) : base(connectionString, Constants.QUEUE_IMAGES)
        {
        }
    }
}
