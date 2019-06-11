using System;

namespace CfpExchange.Models
{
    public class DownloadEventImage
    {
        public Guid Id { get; set; }

        public string ImageUrl { get; set; }

        public bool HasDefaultImage()
        {
            return ImageUrl.EndsWith("noimage.svg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
