using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using DownloadEventImageModel = CfpExchange.Models.DownloadEventImage;

namespace CfpExchange.Functions
{
    public static class DownloadEventImage
    {
        [FunctionName("DownloadEventImage")]
        public static async Task Run(
            [ServiceBusTrigger("eventimages", Connection = "ServicebusQueueConnectionString")]
            string eventImageMessage, Binder binder, ILogger log)
        {
            var eventImageModel = JsonConvert.DeserializeObject<DownloadEventImageModel>(eventImageMessage);
            log.LogInformation($"Processing the download event image for identifier `{eventImageModel.Id}`");

            if (Uri.IsWellFormedUriString(eventImageModel.ImageUrl, UriKind.Absolute))
            {
                log.LogInformation($"Event image URL is `{eventImageModel.ImageUrl}`.");
                var relativeLocationOfStoredImage = await StoreEventImageInBlobStorage(binder, log, eventImageModel);
                if (!eventImageModel.HasDefaultImage())
                {
                    UpdateRecordInTheCfpRepository(eventImageModel, relativeLocationOfStoredImage, log);
                }
            }
            else
            {
                log.LogWarning($"The location `{eventImageModel.ImageUrl}` for CFP `{eventImageModel.Id}` is not an absolute URI.");
            }

            log.LogInformation($"Processed the download event image for identifier `{eventImageModel.Id}`");
        }

        private static async Task<string> StoreEventImageInBlobStorage(
            Binder binder,
            ILogger log,
            DownloadEventImageModel eventImageModel)
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
                log.LogInformation($"Writing event image for CFP `{eventImageModel.Id}` to location `{downloadLocationForEventImage}`.");
                await blobBinding.WriteAsync(imageBytes, 0, imageBytes.Length);

                return downloadLocationForEventImage;
            }
        }

        private static void UpdateRecordInTheCfpRepository(
            DownloadEventImageModel eventImageModel,
            string relativeLocationOfStoredImage,
            ILogger log)
        {
            var storageAccountName = GetEnvironmentVariable("StorageAccountName");
            var absoluteImageLocation = $"https://{storageAccountName}.blob.core.windows.net/{relativeLocationOfStoredImage}";

            var connectionstring = GetEnvironmentVariable("CfpExchangeDb");

            log.LogInformation($"Updating the record `{eventImageModel.Id}` with the event image url to `{absoluteImageLocation}`.");
            using (var connection = new SqlConnection(connectionstring))
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "UPDATE dbo.Cfps SET EventImage = @EventImage WHERE Id = @Id";
                command.Parameters.AddWithValue("EventImage", absoluteImageLocation);
                command.Parameters.AddWithValue("Id", eventImageModel.Id);
                connection.Open();
                command.ExecuteNonQuery();
                log.LogInformation($"Updated the record `{eventImageModel.Id}` with the event image url to `{absoluteImageLocation}`.");
            }
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
