using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace CfpExchange.Functions
{
    public static class DownloadEventImage
    {
        [FunctionName("DownloadEventImage")]
        public static void Run(
            [ServiceBusTrigger("eventimages", AccessRights.Manage, Connection = "ServicebusQueueConnectionString")]
            string myQueueItem, TraceWriter log)
        {
            log.Info($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
