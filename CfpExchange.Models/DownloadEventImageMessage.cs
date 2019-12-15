using System;

namespace CfpExchange.Models
{
    public class DownloadEventImageMessage
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public bool HasDefaultImage()
        {
            return ImageUrl.EndsWith("noimage.svg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
