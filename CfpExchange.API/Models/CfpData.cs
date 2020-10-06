using System;
using System.ComponentModel.DataAnnotations;

namespace CfpExchange.API.Models
{
    public class CfpData
    {
        public Guid Id { get; set; }

        public string EventName { get; set; }

        public string EventDescription { get; set; }

        [Required]
        public DateTime? CfpStartDate { get; set; }

        [Required]
        public DateTime? CfpEndDate { get; set; }
    }
}
