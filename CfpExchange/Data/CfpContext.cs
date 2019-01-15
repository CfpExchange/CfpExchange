using System;
using System.Linq;
using CfpExchange.Helpers;
using CfpExchange.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CfpExchange.Data
{
	public class CfpContext : IdentityDbContext<ApplicationUser>
	{
	    public CfpContext()
	    {
	        
	    }

		public CfpContext(DbContextOptions<CfpContext> options)
			: base(options)
		{
		}

		public virtual DbSet<Cfp> Cfps { get; set; }
	}

	public static class CfpContextExtensions
	{
		public static void EnsureSeeded(this CfpContext context)
		{
			if (context.Cfps.CountAsync().Result == 0)
			{
				context.Cfps.Add(new Cfp
				{
					EventName = "Techorama BE",
					Id = Guid.NewGuid(),
					Views = 100,
					CfpAdded = DateTime.Now.AddDays(-10),
					SubmittedByName = "Gerald",
					EventLocationName = "Antwerp, Belgium",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddDays(100),
					CfpUrl = "http://www.example.com",
					EventUrl = "https://www.techorama.be",
					EventImage = "https://techorama.nl/wp-content/uploads/sites/2/2017/10/TVrobot@4x.svg",
					EventStartDate = DateTime.Now.AddDays(1),
					EventEndDate = DateTime.Now.AddDays(2),
					ProvidesAccommodation = Enums.Accommodation.Unknown,
					ProvidesTravelAssistance = Enums.TravelAssistence.Yes,
					Slug = "techorama-be",
					EventTags = ".NET;Awesome;Unicorns"
				});

				context.Cfps.Add(new Cfp
				{
					EventName = "Techorama NL",
					Id = Guid.NewGuid(),
					Views = 10,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Ede, The Netherlands",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddDays(10),
					EventImage = "https://techorama.nl/wp-content/uploads/sites/2/2017/10/TVrobot@4x.svg",
					EventStartDate = DateTime.Now.AddMonths(1),
					EventEndDate = DateTime.Now.AddMonths(1).AddDays(2),
					ProvidesAccommodation = Enums.Accommodation.No,
					ProvidesTravelAssistance = Enums.TravelAssistence.Unknown,
					Slug = "techorama-nl",
					EventTags = ".NET;Awesome;Unicorns;Dutch"
				});

				context.Cfps.Add(new Cfp
				{
					EventName = "NDC Minnesota",
					Id = Guid.NewGuid(),
					Views = 15,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Minnesota, USA",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddDays(1),
					ProvidesAccommodation = Enums.Accommodation.Yes,
					ProvidesTravelAssistance = Enums.TravelAssistence.Yes,
					Slug = "ndc-minnesota",
					EventTags = ".NET;Awesome;Unicorns;PubConf"
				});

				// Give this one a static GUID to be able to access it
				context.Cfps.Add(new Cfp
				{
					EventName = "NDC London",
					Id = new Guid("d238d46f-055a-44fd-ad99-840a4741635f"),
					Views = 23,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "London, UK",
					EventLocationLat = 51.5073509,
					EventLocationLng = -0.127758299999982,
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit2.",
					CfpEndDate = DateTime.Now.AddHours(-24),
					ProvidesAccommodation = Enums.Accommodation.No,
					ProvidesTravelAssistance = Enums.TravelAssistence.No,
					Slug = "ndc-london",
					EventTags = ".NET;Awesome;Unicorns;Bla"
				});

				context.Cfps.Add(new Cfp
				{
					EventName = "NDC Oslo",
					Id = Guid.NewGuid(),
					Views = 34,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Oslo, Norway",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddDays(20),
					EventTags = ".NET;Awesome;Unicorns;Cold"
				});

				// Give this one a static GUID to be able to access it
				context.Cfps.Add(new Cfp
				{
					EventName = "SDN Event",
					Id = new Guid("dd3f7150-c9a7-46ed-9481-9246aed2329d"),
					Views = 42,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Zeist, The Netherlands",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit2.",
					CfpEndDate = DateTime.Now.AddDays(7),
					ProvidesAccommodation = Enums.Accommodation.Unknown,
					ProvidesTravelAssistance = Enums.TravelAssistence.Unknown,
					Slug = "sdn-event",
					EventTags = ""
				});

				// Give this one a static GUID to be able to access it
				context.Cfps.Add(new Cfp
				{
					EventName = "SDN Event duplicate",
					Id = new Guid("30eb1e5c-5959-4685-b2bb-36d816023007"),
					Views = 3,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Zeist, The Netherlands",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit1.",
					CfpEndDate = DateTime.Now.AddDays(4),
					ProvidesAccommodation = Enums.Accommodation.Unknown,
					ProvidesTravelAssistance = Enums.TravelAssistence.Unknown,
					DuplicateOfId = new Guid("dd3f7150-c9a7-46ed-9481-9246aed2329d"),
					Slug = "sdn-event-1",
					EventTags = ".NET;Awesome;Unicorns;SDN"
				});

				context.SaveChanges();
			}

			// Ensure all cfps have a slug
			var cfpsWithoutSlug = context.Cfps.Where(cfp => string.IsNullOrWhiteSpace(cfp.Slug));

			if (cfpsWithoutSlug.Any())
			{
				foreach (var cfp in cfpsWithoutSlug)
					cfp.Slug = FriendlyUrlHelper.GetFriendlyTitle(cfp.EventName);

				context.SaveChanges();
			}
		}
	}
}
