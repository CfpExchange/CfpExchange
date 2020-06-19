using System.Collections.Generic;
using CfpExchange.Common.Models;

namespace CfpExchange.ViewModels
{
	public class BrowseResponseViewModel
	{
		public IEnumerable<Cfp> CfpList { get; }
		public int CurrentPage { get; }
		public string SearchTerm { get; }
		public string EventDate { get; }

		public BrowseResponseViewModel(IEnumerable<Cfp> cfpList, int currentPage, string searchTerm = "", string eventdate = "0")
		{
			CfpList = cfpList;
			CurrentPage = currentPage;
			SearchTerm = searchTerm;
			EventDate = eventdate;
		}
	}
}