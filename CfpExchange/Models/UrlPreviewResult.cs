using Newtonsoft.Json;

namespace CfpExchange.Models
{
	public partial class UrlPreviewResult
	{
		[JsonProperty("_type")]
		public string Type { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("isFamilyFriendly")]
		public bool IsFamilyFriendly { get; set; }

		[JsonProperty("primaryImageOfPage")]
		public PrimaryImageOfPage PrimaryImageOfPage { get; set; }
	}

	public partial class PrimaryImageOfPage
	{
		[JsonProperty("contentUrl")]
		public string ContentUrl { get; set; }
	}
}