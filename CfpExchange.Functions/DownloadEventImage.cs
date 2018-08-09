using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace CfpExchange.Functions
{
    public static class DownloadEventImage
    {
        [FunctionName("DownloadEventImage")]
        public static async Task Run(
            [ServiceBusTrigger("eventimages", AccessRights.Manage, Connection = "ServicebusQueueConnectionString")]
            string eventImageMessage,
            Binder binder,
            TraceWriter log)
        {
            var eventImageModel = JsonConvert.DeserializeObject<Models.DownloadEventImage>(eventImageMessage);
            log.Info($"Processing the download event image for identifier `{eventImageModel.Id}`");

            if (Uri.IsWellFormedUriString(eventImageModel.ImageUrl, UriKind.Absolute))
            {
                log.Verbose($"Event image URL is `{eventImageModel.ImageUrl}`.");
                Uri uri = new Uri(eventImageModel.ImageUrl);
                var filename = Path.GetFileName(uri.LocalPath);
                var downloadLocationForEventImage = $"eventimages/{eventImageModel.Id}/{filename}";

                using (var blobBinding = await binder.BindAsync<Stream>(
                    new BlobAttribute(downloadLocationForEventImage, FileAccess.Write)))
                {
                    var webClient = new WebClient();
                    var imageBytes = await webClient.DownloadDataTaskAsync(uri);
                    log.Verbose($"Writing event image for CFP `{eventImageModel.Id}` to location `{downloadLocationForEventImage}`.");
                    await blobBinding.WriteAsync(imageBytes, 0, imageBytes.Length);
                }
            }
            else
            {
                log.Warning($"The location `{eventImageModel.ImageUrl}` for CFP `{eventImageModel.Id}` is not an absolute URI.");
            }
            
            log.Info($"Processed the download event image for identifier `{eventImageModel.Id}`");
        }
    }
}
