using System;

namespace CfpExchange.Common.Messages
{
    public class DownloadEventImageMessage
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool HasDefaultImage()
        {
            return ImageUrl.EndsWith("noimage.svg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
