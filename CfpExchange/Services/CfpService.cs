using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CfpExchange.Data;
using CfpExchange.Models;

namespace CfpExchange.Services
{
    public class CfpService : ICfpService
    {
        private const int MaximumNumberOfItemsPerPage = 10;
        
        private readonly CfpContext _cfpContext;

        public CfpService(CfpContext cfpContext)
        {
            _cfpContext = cfpContext;
        }

        public Cfp GetCfpById(Guid id)
        {
            return _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Id == id);
        }

        public Cfp GetCfpBySlug(string slug)
        {
            return _cfpContext.Cfps.SingleOrDefault(cfp => cfp.Slug == slug);
        }

        public List<Cfp> GetAllActiveCfps(string lowercaseSearchTerm, DateTime startDateTime, DateTime endDateTime, int pageToShow)
        {
            var allActiveCfps = _cfpContext.Cfps
                .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                .Where(cfp => cfp.DuplicateOfId == null)
                .Where(cfp => cfp.EventName.ToLower().Contains(lowercaseSearchTerm)
                              || cfp.EventLocationName.ToLower().Contains(lowercaseSearchTerm)
                              || cfp.EventTags.ToLower().Contains(lowercaseSearchTerm))
                .Where(cfp => cfp.EventStartDate == default || cfp.EventEndDate == default ||
                              cfp.EventStartDate >= startDateTime && cfp.EventEndDate <= endDateTime)
                .OrderBy(cfp => cfp.CfpEndDate)
                .Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
                .Take(MaximumNumberOfItemsPerPage)
                .ToList();

            return allActiveCfps;
        }

        public List<Cfp> GetNewestActiveCfps(int pageToShow)
        {
            var allActiveCfps = _cfpContext.Cfps
                .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                .Where(cfp => cfp.DuplicateOfId == null)
                .OrderByDescending(cfp => cfp.CfpAdded.Date)
                .ThenBy(cfp => cfp.CfpEndDate.Date)
                .Skip((pageToShow - 1) * MaximumNumberOfItemsPerPage)
                .Take(MaximumNumberOfItemsPerPage)
                .ToList();
            return allActiveCfps;
        }

        public List<Cfp> IsAlreadyInDatabase(Uri parsedUri)
        {
            var alreadyInDatabase = _cfpContext.Cfps
                .Where(cfp => cfp.CfpEndDate > DateTime.UtcNow)
                .Where(c => c.EventUrl.ToLowerInvariant().Contains(parsedUri.Host) ||
                            c.CfpUrl.ToLowerInvariant().Contains(parsedUri.Host))
                .ToList();

            return alreadyInDatabase;
        }

        public bool CfpWithIdenticalSlugExists(string cfpToAddSlug)
        {
            return _cfpContext.Cfps.Any(cfp => cfp.Slug == cfpToAddSlug);
        }

        public void AddCfp(Cfp cfpToAdd)
        {
            _cfpContext.Add(cfpToAdd);
        }

        public async Task SaveChangesAsync()
        {
            await _cfpContext.SaveChangesAsync();
        }
    }
}
