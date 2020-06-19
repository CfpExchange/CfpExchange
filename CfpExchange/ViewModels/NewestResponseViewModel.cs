using System.Collections.Generic;
using CfpExchange.Common.Models;

namespace CfpExchange.ViewModels
{
    public class NewestResponseViewModel
    {
        public IEnumerable<Cfp> CfpList { get; }
        public int CurrentPage { get; }

        public NewestResponseViewModel(IEnumerable<Cfp> cfpList, int currentPage)
        {
            CfpList = cfpList;
            CurrentPage = currentPage;
        }
    }
}