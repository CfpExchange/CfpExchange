using System;
using System.ComponentModel.DataAnnotations;
using CfpExchange.Enums;
using CfpExchange.Helpers;

namespace CfpExchange.Models
{
	public class Cfp
	{
		[Key]
		public Guid Id { get; set; }
		public string EventName { get; set; }
		public string EventDescription { get; set; }
		public string EventLocationName { get; set; }
		public double EventLocationLat { get; set; }
		public double EventLocationLng { get; set; }
		public string EventUrl { get; set; } = String.Empty;

		private string _eventImage;
		public string EventImage
		{
			get
			{
				return string.IsNullOrWhiteSpace(_eventImage) ? Constants.NoEventImageUrl : _eventImage;
			}

			set
			{
				_eventImage = value;
			}
		}

		public DateTime EventStartDate { get; set; }
		public DateTime EventEndDate { get; set; }
		public DateTime CfpStartDate { get; set; }
		public DateTime CfpEndDate { get; set; }
		public DateTime CfpDecisionDate { get; set; }
		public DateTime CfpAdded { get; set; }
		public Accommodation ProvidesAccommodation { get; set; }
		public TravelAssistence ProvidesTravelAssistance { get; set; }
		public string CfpUrl { get; set; } = String.Empty;
		public string Remarks { get; set; }
		public int Views { get; set; }
		public int ClicksToCfpUrl { get; set; }
		public string SubmittedByName { get; set; }
	}
}