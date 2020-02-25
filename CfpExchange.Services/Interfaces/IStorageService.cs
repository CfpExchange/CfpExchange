using System;
using System.Threading.Tasks;

namespace CfpExchange.Services.Interfaces
{
    public interface IStorageService
    {
        Task<string> StoreEventImageInBlobStorageAsync(string url, Guid id);

        void UpdateRecordInTheCfpRepository(Guid id, string relativeLocationOfStoredImage);
    }
}
