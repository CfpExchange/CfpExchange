using System;

namespace CfpExchange.Common.Models
{
    public class SendTweetMessage
    {
        public string UrlToCfp { get; set; } = string.Empty;

        public string TwitterHandle { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public DateTime CfpEndDate { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public decimal EventLocationLatitude { get; set; }

        public decimal EventLocationLongitude { get; set; }
    }
}
