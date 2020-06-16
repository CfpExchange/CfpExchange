using System;
using System.ComponentModel.DataAnnotations;

namespace CfpExchange.ViewModels
{
	public class IssueViewModel
	{
		[Required]
		public Guid CfpId { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }
	}
}
