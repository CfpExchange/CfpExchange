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
			Database.Migrate();
		}

		public DbSet<Cfp> Cfps { get; set; }
	}
}