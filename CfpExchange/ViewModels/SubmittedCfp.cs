using System;
using System.ComponentModel.DataAnnotations;
using CfpExchange.Enums;

namespace CfpExchange.ViewModels
{
	public class SubmittedCfp
	{
		[Required(ErrorMessage ="The event URL is required")]
		[DataType(DataType.Url)]
		public string EventUrl { get; set; }

		[DataType(DataType.ImageUrl)]
		public string EventImageUrl { get; set; }

		[Required(ErrorMessage = "The event title is required")]
		[DataType(DataType.Text)]
		public string EventTitle { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string EventTags { get; set; }

        [Required(ErrorMessage = "A description is required for the event")]
		[DataType(DataType.MultilineText)]
		public string EventDescription { get; set; }

		[DataType(DataType.Text)]
		public string LocationName { get; set; }
		public double LocationLat { get; set; }
		public double LocationLng { get; set; }

		[Required(ErrorMessage ="The CFP end date is required")]
		[DataType(DataType.Date)]
		public DateTime CfpEndDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime? EventStartDate { get; set; } = default(DateTime);

		[DataType(DataType.Date)]
		public DateTime? EventEndDate { get; set; } = default(DateTime);

		[DataType(DataType.Url)]
		public string CfpUrl { get; set; }

		[EnumDataType(typeof(Accommodation))]
		public Accommodation ProvidesAccommodation { get; set; }

		[EnumDataType(typeof(TravelAssistence))]
		public TravelAssistence ProvidesTravelAssistance { get; set; }

		[DataType(DataType.Text)]
		public string SubmittedByName { get; set; }

		[DataType(DataType.Text)]
		public string EventTwitterHandle { get; set; }

	    [DataType(DataType.Date)]
	    public DateTime? CfpDecisionDate { get; set; } = default(DateTime);
    }
}