using System.Collections.Generic;
using CfpExchange.Models;

namespace CfpExchange.ViewModels
{
    public class BrowseResponseViewModel
    {
        public IEnumerable<Cfp> CfpList { get; }
        public int CurrentPage { get; }

        public BrowseResponseViewModel(IEnumerable<Cfp> cfpList, int currentPage)
        {
            CfpList = cfpList;
            CurrentPage = currentPage;
        }
    }
}
