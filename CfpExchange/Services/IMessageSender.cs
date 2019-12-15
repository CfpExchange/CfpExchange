using System;
using System.Threading.Tasks;
using CfpExchange.Models;

namespace CfpExchange.Services
{
    public interface IMessageSender
    {
        Task SendDownloadEventImageMessageAsync(Guid cfpPublicId, string cfpEventImageUrl);

        Task SendTwitterMessageAsync(Cfp cfpToAdd, string urlToCfp);
    }
}
