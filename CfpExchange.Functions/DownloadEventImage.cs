using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using CfpExchange.Models;
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
                var relativeLocationOfStoredImage = await StoreEventImageInBlobStorage(binder, log, eventImageModel);
                if (!eventImageModel.ImageUrl.IsDefaultImage())
                {
                    UpdateRecordInTheCfpRepository(eventImageModel, relativeLocationOfStoredImage, log);
                }
            }
            else
            {
                log.Warning($"The location `{eventImageModel.ImageUrl}` for CFP `{eventImageModel.Id}` is not an absolute URI.");
            }

            log.Info($"Processed the download event image for identifier `{eventImageModel.Id}`");
        }

        private static async Task<string> StoreEventImageInBlobStorage(
            Binder binder,
            TraceWriter log,
            Models.DownloadEventImage eventImageModel)
        {
            Uri uri = new Uri(eventImageModel.ImageUrl);
            var filename = Path.GetFileName(uri.LocalPath);
            var downloadLocationForEventImage = $"eventimages/{eventImageModel.Id}/{filename}";

            using (var blobBinding = await binder.BindAsync<Stream>(
                new Attribute[] {new BlobAttribute(downloadLocationForEventImage, FileAccess.Write),
                new StorageAccountAttribute("StorageAccountConnectionString")}))

        {
                var webClient = new WebClient();
                var imageBytes = await webClient.DownloadDataTaskAsync(uri);
                log.Verbose($"Writing event image for CFP `{eventImageModel.Id}` to location `{downloadLocationForEventImage}`.");
                await blobBinding.WriteAsync(imageBytes, 0, imageBytes.Length);

                return downloadLocationForEventImage;
            }
        }

        private static void UpdateRecordInTheCfpRepository(
            Models.DownloadEventImage eventImageModel,
            string relativeLocationOfStoredImage,
            TraceWriter log)
        {
            var storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
            var absoluteImageLocation = $"https://{storageAccountName}.blob.core.windows.net/{relativeLocationOfStoredImage}";

            var connectionstring = ConfigurationManager.AppSettings["CfpExchangeDb"];

            log.Info($"Updating the record `{eventImageModel.Id}` with the event image url to `{absoluteImageLocation}`.");
            using (var connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                connection.Execute("UPDATE dbo.Cfps SET EventImage = @EventImage WHERE Id = @Id",
                    new
                    {
                        EventImage = absoluteImageLocation,
                        Id = eventImageModel.Id.ToString("D")
                    });
                log.Info($"Updated the record `{eventImageModel.Id}` with the event image url to `{absoluteImageLocation}`.");
            }
        }
    }
}
