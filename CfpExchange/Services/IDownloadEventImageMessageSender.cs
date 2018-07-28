using System;
using System.Threading.Tasks;

namespace CfpExchange.Services
{
    public interface IDownloadEventImageMessageSender
    {
        Task Execute(Guid cfpPublicId, string cfpEventImageUrl);
    }
}