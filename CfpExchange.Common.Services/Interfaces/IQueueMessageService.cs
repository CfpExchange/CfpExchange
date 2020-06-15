using System;
using System.Threading.Tasks;

using CfpExchange.Common.Models;

namespace CfpExchange.Common.Services.Interfaces
{
    public interface IQueueMessageService
    {
        Task SendDownloadEventImageMessageAsync(Guid cfpPublicId, string cfpEventImageUrl);

        Task SendTwitterMessageAsync(CfpInformation cfp, string urlToCfp);
    }
}
