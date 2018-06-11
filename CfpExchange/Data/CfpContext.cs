using System;
using CfpExchange.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CfpExchange.Data
{
	public class CfpContext : IdentityDbContext<ApplicationUser>
	{
		public CfpContext(DbContextOptions<CfpContext> options)
			: base(options)
		{
		}

		public DbSet<Cfp> Cfps { get; set; }
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
					CfpUrl = "www.example.com",
					EventUrl = "https://www.techorama.be",
					EventImage = "https://techorama.nl/wp-content/uploads/sites/2/2017/10/TVrobot@4x.svg",
					EventStartDate = DateTime.Now.AddDays(1),
					EventEndDate = DateTime.Now.AddDays(2),
					ProvidedExpenses = Enums.Expenses.Reimbursements
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
					EventStartDate = DateTime.Now.AddDays(1),
					EventEndDate = DateTime.Now.AddDays(1),
					ProvidedExpenses = Enums.Expenses.None
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
					CfpEndDate = DateTime.Now.AddDays(1)
				});

				context.Cfps.Add(new Cfp
				{
					EventName = "NDC London",
					Id = Guid.NewGuid(),
					Views = 23,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "London, UK",
					EventLocationLat = 51.5073509,
					EventLocationLng = -0.127758299999982,
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddHours(2)
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
					CfpEndDate = DateTime.Now.AddDays(20)
				});

				context.Cfps.Add(new Cfp
				{
					EventName = "SDN Event",
					Id = Guid.NewGuid(),
					Views = 42,
					CfpAdded = DateTime.Now,
					SubmittedByName = "Gerald",
					EventLocationName = "Zeist, The Netherlands",
					EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
					CfpEndDate = DateTime.Now.AddDays(7),
					ProvidedExpenses = Enums.Expenses.Unknown
				});

				context.SaveChanges();
			}
		}
	}
}