using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using CfpExchange.Common.Services.Interfaces;

namespace CfpExchange.Common.Services
{
    public class StorageService : BaseService, IStorageService
    {
        #region Constants

        private const string CONTAINER_NAME = "eventimages";

        #endregion

        #region Fields

        private HttpClient _httpClient;

        #endregion

        #region Constructors

        public StorageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        #endregion

        public async Task<string> StoreEventImageInBlobStorageAsync(string url, Guid id)
        {
            var uri = new Uri(url);
            var filename = Path.GetFileName(uri.LocalPath);
            var downloadLocationForEventImage = $"{id}/{filename}";

            var connectionString = GetEnvironmentVariable("StorageAccountConnectionString");
            var container = await new BlobServiceClient(connectionString).CreateBlobContainerAsync(CONTAINER_NAME);
            var image = await (await _httpClient.GetAsync(url)).Content.ReadAsStreamAsync();
            _ = await container.Value.UploadBlobAsync(downloadLocationForEventImage, image);

            return $"{CONTAINER_NAME}/{downloadLocationForEventImage}";
        }

        public void UpdateRecordInTheCfpRepository(Guid id, string relativeLocationOfStoredImage)
        {
            var storageAccountName = GetEnvironmentVariable("StorageAccountName");
            var absoluteImageLocation = $"https://{storageAccountName}.blob.core.windows.net/{relativeLocationOfStoredImage}";
            var connectionstring = GetEnvironmentVariable("CfpExchangeDb");

            using var connection = new SqlConnection(connectionstring);
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "UPDATE dbo.Cfps SET EventImage = @EventImage WHERE Id = @Id";
            command.Parameters.AddWithValue("EventImage", absoluteImageLocation);
            command.Parameters.AddWithValue("Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
