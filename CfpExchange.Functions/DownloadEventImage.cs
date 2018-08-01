using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;

namespace CfpExchange.Functions
{
    public static class DownloadEventImage
    {
        [FunctionName("DownloadEventImage")]
        public static async Task Run(
            [ServiceBusTrigger("eventimages", AccessRights.Manage, Connection = "ServicebusQueueConnectionString")]
            Model.DownloadEventImage eventImageModel,
            Binder binder,
            TraceWriter log)
        {
            log.Info($"Processing the download event image for identifier `{eventImageModel.Id}`");

            Uri uri = new Uri(eventImageModel.ImageUrl);
            var filename = Path.GetFileName(uri.LocalPath);
            var downloadLocationForEventImage = $"event-images/{eventImageModel.Id}/{filename}";

            using (var blobBinding = await binder.BindAsync<BinaryWriter>(
                                                new BlobAttribute(downloadLocationForEventImage, FileAccess.Write)))
            {
                var webClient = new WebClient();
                var imageBytes = await webClient.DownloadDataTaskAsync(uri);
                blobBinding.Write(imageBytes);
            }

            log.Info($"Processed the download event image for identifier `{eventImageModel.Id}`");
        }
    }
}
