using Microsoft.Azure.ServiceBus;

using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class TwitterQueueClient : QueueClient, ITwitterQueueClient
    {
        public TwitterQueueClient(string connectionString) : base(connectionString, Constants.QUEUE_TWITTER) 
        {
        }
    }
}
