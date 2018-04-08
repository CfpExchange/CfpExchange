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

			SaveChanges();
		}

		public DbSet<Cfp> Cfps { get; set; }
	}
}