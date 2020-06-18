using System.Net;

namespace CfpExchange.Common.Models
{
	public class MetaInformation
	{
		private string _title;
		private string _description;

		public bool HasError { get; set; }
		public bool ExternalPageError { get; set; }

		public bool HasData => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Description)
								|| !string.IsNullOrWhiteSpace(ImageUrl);
		public string Url { get; set; }

		public string Title
		{
			get
			{
				return WebUtility.HtmlDecode(_title);
			}

			set
			{
				_title = value;
			}
		}

		public string Description
		{
			get
			{
				return WebUtility.HtmlDecode(_description);
			}

			set
			{
				_description = value;
			}
		}

		public string Keywords { get; set; }
		public string ImageUrl { get; set; }
		public string SiteName { get; set; }

		public MetaInformation(string url)
		{
			Url = url;
		}

		public MetaInformation(string url, string title, string description, string keywords, string imageUrl, string siteName)
		{
			Url = url;
			Title = title;
			Description = description;
			Keywords = keywords;
			ImageUrl = imageUrl;
			SiteName = siteName;
		}
	}
}
