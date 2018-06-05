using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using CfpExchange.Models;
using HtmlAgilityPack;

namespace CfpExchange.Helpers
{
	// Taken from https://codeshare.co.uk/blog/how-to-scrape-meta-data-from-a-url-using-htmlagilitypack-in-c/
	// And modified
	public static class MetaScraper
	{
		/// <summary>
		/// Uses HtmlAgilityPack to get the meta information from a url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static MetaInformation GetMetaDataFromUrl(string url)
		{
			var metaInfo = new MetaInformation(url);

			try
			{
				// Get the URL specified
				var webGet = new HtmlWeb();
				var document = webGet.Load(url);
				var metaTags = document.DocumentNode.SelectNodes("//meta");

				if (metaTags != null)
				{
					foreach (var tag in metaTags)
					{
						var tagName = tag.Attributes["name"];
						var tagContent = tag.Attributes["content"];
						var tagProperty = tag.Attributes["property"];

						if (tagName != null && tagContent != null)
						{
							switch (tagName.Value.ToLower())
							{
								case "title":
									metaInfo.Title = tagContent.Value;
									break;
								case "description":
									metaInfo.Description = tagContent.Value;
									break;
								case "twitter:title":
									metaInfo.Title = string.IsNullOrEmpty(metaInfo.Title) ? tagContent.Value : metaInfo.Title;
									break;
								case "twitter:description":
									metaInfo.Description = string.IsNullOrEmpty(metaInfo.Description) ? tagContent.Value : metaInfo.Description;
									break;
								case "keywords":
									metaInfo.Keywords = tagContent.Value;
									break;
								case "twitter:image":
									metaInfo.ImageUrl = string.IsNullOrEmpty(metaInfo.ImageUrl) ? tagContent.Value : metaInfo.ImageUrl;
									break;
							}
						}
						else if (tagProperty != null && tagContent != null)
						{
							switch (tagProperty.Value.ToLower())
							{
								case "og:title":
									metaInfo.Title = string.IsNullOrEmpty(metaInfo.Title) ? tagContent.Value : metaInfo.Title;
									break;
								case "og:description":
									metaInfo.Description = string.IsNullOrEmpty(metaInfo.Description) ? tagContent.Value : metaInfo.Description;
									break;
								case "og:image":
									metaInfo.ImageUrl = string.IsNullOrEmpty(metaInfo.ImageUrl) ? tagContent.Value : metaInfo.ImageUrl;
									break;
							}
						}
					}

					if (string.IsNullOrWhiteSpace(metaInfo.Title))
					{
						metaInfo.Title = GetTitle(document.ParsedText);
					}
				}
			}
			catch (Exception ex)
			{
				// TODO: We should probably log this somewhere...
				metaInfo.HasError = true;

				metaInfo.ExternalPageError = ex is WebException;
			}

			return metaInfo;
		}

		private static string GetTitle(string s)
		{
			Match m = Regex.Match(s, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>");
			if (m.Success)
			{
				return m.Groups[1].Value;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}