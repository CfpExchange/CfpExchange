using System;

namespace CfpExchange.Services
{
    public interface IDownloadEventImageMessageSender
    {
        void Execute(Guid cfpPublicId, string cfpEventImageUrl);
    }
}