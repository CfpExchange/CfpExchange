using System;

namespace CfpExchange.Common.Models
{
    public class CfpInformation
    {
        public DateTime CfpEndDate { get; set; }

        public DateTime EventStartDate { get; set; }

        public DateTime EventEndDate { get; set; }

        public decimal EventLocationLatitude { get; set; }

        public decimal EventLocationLongitude { get; set; }

        public string EventName { get; set; }

        public string TwitterHandle { get; set; }

        public string UrlToCfp { get; set; }
    }
}
