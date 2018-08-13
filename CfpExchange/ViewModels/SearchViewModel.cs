namespace CfpExchange.ViewModels
{
	public class SearchViewModel
	{
		public string SearchTerm { get; }
		public string EventDate { get; }

		public SearchViewModel(string searchTerm = "", string eventdate = "0")
		{
			SearchTerm = searchTerm;
			EventDate = eventdate;
		}
	}
}