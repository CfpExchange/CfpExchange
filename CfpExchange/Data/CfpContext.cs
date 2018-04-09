using System;
using CfpExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CfpExchange.Data
{
	public class CfpContext : DbContext
	{
		public CfpContext(DbContextOptions<CfpContext> options)
			: base(options)
		{
			if (Cfps.CountAsync().Result == 0)
			{
				Cfps.Add(new Cfp
				{
					EventName = "Techorama BE",
					Id = Guid.NewGuid(),
					Views = 100,
					CfpAdded = DateTime.Now.AddDays(-10)
				});

				Cfps.Add(new Cfp
				{
					EventName = "Techorama NL",
					Id = Guid.NewGuid(),
					Views = 10,
					CfpAdded = DateTime.Now
				});

				Cfps.Add(new Cfp
				{
					EventName = "NDC Minnesota",
					Id = Guid.NewGuid(),
					Views = 15,
					CfpAdded = DateTime.Now
				});

				Cfps.Add(new Cfp
				{
					EventName = "NDC London",
					Id = Guid.NewGuid(),
					Views = 23,
					CfpAdded = DateTime.Now
				});

				Cfps.Add(new Cfp
				{
					EventName = "NDC Oslo",
					Id = Guid.NewGuid(),
					Views = 34,
					CfpAdded = DateTime.Now
				});

				Cfps.Add(new Cfp
				{
					EventName = "SDN Event",
					Id = Guid.NewGuid(),
					Views = 42,
					CfpAdded = DateTime.Now
				});

				SaveChanges();
			}
		}

		public DbSet<Cfp> Cfps { get; set; }
	}
}