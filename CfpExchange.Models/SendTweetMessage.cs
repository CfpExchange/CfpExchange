using System;

namespace CfpExchange.Models
{
    public class SendTweetMessage
    {
        public string UrlToCfp { get; set; }

        public string TwitterHandle { get; set; }

        public string EventName { get; set; }

        public DateTime CfpEndDate { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public decimal EventLocationLatitude { get; set; }

        public decimal EventLocationLongitude { get; set; }
    }
}
