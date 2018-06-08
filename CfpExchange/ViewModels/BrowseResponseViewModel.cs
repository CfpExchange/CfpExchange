using System.Collections.Generic;
using CfpExchange.Models;

namespace CfpExchange.ViewModels
{
    public class BrowseResponseViewModel
    {
        public IEnumerable<Cfp> CfpList { get; }
		public int CurrentPage { get; }
        public string SearchTerm { get; }

        public BrowseResponseViewModel(IEnumerable<Cfp> cfpList, int currentPage, string searchTerm = "")
        {
            CfpList = cfpList;
            CurrentPage = currentPage;
			SearchTerm = searchTerm;
        }
    }
}