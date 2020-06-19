using CfpExchange.Common.Models;

namespace CfpExchange.ViewModels
{
    public class IndexViewModel
	{
		public Cfp NewestCfp { get; set; }
		public Cfp MostViewedCfp { get; set; }
		public Cfp RandomCfp { get; set; }
		public Cfp CfpOfTheDay { get; set; }

		public Cfp[] CfpList { get; set; }
	}
}