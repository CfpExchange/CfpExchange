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
}