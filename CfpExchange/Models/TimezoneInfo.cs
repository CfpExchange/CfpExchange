using System;
using Newtonsoft.Json;

namespace CfpExchange.Models
{
	public class TimezoneInfo
	{
		[JsonProperty("Version")]
		public string Version { get; set; }

		[JsonProperty("ReferenceUtcTimestamp")]
		public DateTimeOffset ReferenceUtcTimestamp { get; set; }

		[JsonProperty("TimeZones")]
		public TimeZone[] TimeZones { get; set; }
	}

	public class TimeZone
	{
		[JsonProperty("Id")]
		public string Id { get; set; }

		[JsonProperty("Names")]
		public Names Names { get; set; }

		[JsonProperty("ReferenceTime")]
		public ReferenceTime ReferenceTime { get; set; }
	}

	public class Names
	{
		[JsonProperty("ISO6391LanguageCode")]
		public string Iso6391LanguageCode { get; set; }

		[JsonProperty("Generic")]
		public string Generic { get; set; }

		[JsonProperty("Standard")]
		public string Standard { get; set; }

		[JsonProperty("Daylight")]
		public string Daylight { get; set; }
	}

	public class ReferenceTime
	{
		[JsonProperty("Tag")]
		public string Tag { get; set; }

		[JsonProperty("StandardOffset")]
		public string StandardOffset { get; set; }

		[JsonProperty("DaylightSavings")]
		public DateTimeOffset DaylightSavings { get; set; }

		[JsonProperty("WallTime")]
		public DateTimeOffset WallTime { get; set; }

		[JsonProperty("PosixTzValidYear")]
		public long PosixTzValidYear { get; set; }

		[JsonProperty("PosixTz")]
		public string PosixTz { get; set; }
	}
}
