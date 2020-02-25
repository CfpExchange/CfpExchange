using System;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using CfpExchange.Models;
using CfpExchange.Services.Interfaces;

namespace CfpExchange.Functions
{
    public class DownloadEventImage : BaseFunction
    {
        #region Fields

        private readonly IStorageService _storageService;

        #endregion

        #region Constructors

        public DownloadEventImage(IStorageService storageService)
        {
            _storageService = storageService;
        }

        #endregion

        [FunctionName(nameof(DownloadEventImage))]
        public async Task Run(
            [ServiceBusTrigger(Constants.QUEUE_IMAGES, Connection = "ServicebusQueueConnectionString")]
            DownloadEventImageMessage eventImageMessage, Binder binder, ILogger log)
        {
            log.LogInformation($"Processing the download event image for identifier `{eventImageMessage.Id}`");

            if (Uri.IsWellFormedUriString(eventImageMessage.ImageUrl, UriKind.Absolute))
            {
                log.LogInformation($"Event image URL is `{eventImageMessage.ImageUrl}`.");
                var relativeLocationOfStoredImage = await _storageService.StoreEventImageInBlobStorageAsync(eventImageMessage.ImageUrl, eventImageMessage.Id);
                if (!eventImageMessage.HasDefaultImage())
                {
                    _storageService.UpdateRecordInTheCfpRepository(eventImageMessage.Id, relativeLocationOfStoredImage);
                }
            }
            else
            {
                log.LogWarning($"The location `{eventImageMessage.ImageUrl}` for CFP `{eventImageMessage.Id}` is not an absolute URI.");
            }

            log.LogInformation($"Done processing the download event image for identifier `{eventImageMessage.Id}`");
        }


    }
}
