using System;

namespace CfpExchange.Models
{
	public class Cfp
	{
		public Guid Id { get; set; }
		public string EventName { get; set; }
		public string EventLocation { get; set; }
		public string EventUrl { get; set; }
		public string EventImage { get; set; }

		public DateTime EventStartDate { get; set; }
		public DateTime EventEndDate { get; set; }
		public DateTime CfpStartDate { get; set; }
		public DateTime CfpEndDate { get; set; }
		public DateTime CfpAdded { get; set; }

		public string CfpUrl { get; set; }

		public int Views { get; set; }
		public int ClicksToCfpUrl { get; set; }
		public string SubmittedByName { get; set; }
	}
}