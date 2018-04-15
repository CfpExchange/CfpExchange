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
			Database.Migrate();
            if (Cfps.CountAsync().Result == 0)
            {
                Cfps.Add(new Cfp
                {
                    EventName = "Techorama BE",
                    Id = Guid.NewGuid(),
                    Views = 100,
                    CfpAdded = DateTime.Now.AddDays(-10),
                    SubmittedByName = "Gerald",
                    EventLocationName = "Antwerp, Belgium",
                    EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    CfpEndDate = DateTime.Now.AddDays(100),
                    EventImage = "https://techorama.nl/wp-content/uploads/sites/2/2017/10/TVrobot@4x.svg"
                });

                Cfps.Add(new Cfp
                {
                    EventName = "Techorama NL",
                    Id = Guid.NewGuid(),
                    Views = 10,
                    CfpAdded = DateTime.Now,
                    SubmittedByName = "Gerald",
                    EventLocationName = "Ede, The Netherlands",
                    EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    CfpEndDate = DateTime.Now.AddDays(10),
                    EventImage = "https://techorama.nl/wp-content/uploads/sites/2/2017/10/TVrobot@4x.svg"
                });

                Cfps.Add(new Cfp
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

                Cfps.Add(new Cfp
                {
                    EventName = "NDC London",
                    Id = Guid.NewGuid(),
                    Views = 23,
                    CfpAdded = DateTime.Now,
                    SubmittedByName = "Gerald",
                    EventLocationName = "London, UK",
                    EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    CfpEndDate = DateTime.Now.AddHours(2)
                });

                Cfps.Add(new Cfp
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

                Cfps.Add(new Cfp
                {
                    EventName = "SDN Event",
                    Id = Guid.NewGuid(),
                    Views = 42,
                    CfpAdded = DateTime.Now,
                    SubmittedByName = "Gerald",
                    EventLocationName = "Zeist, The Netherlands",
                    EventDescription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                    CfpEndDate = DateTime.Now.AddDays(7)
                });

                SaveChanges();
            }
		}

		public DbSet<Cfp> Cfps { get; set; }
	}
}