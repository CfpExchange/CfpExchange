using OpenQA.Selenium.Remote;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class HomePage : SharedPageActions
    {
        public HomePage(RemoteWebDriver driver) : base(driver)
        {
        }

        public CfpSingleItemPage ShowsNewestCfp()
        {
            var webElement = Driver.FindElementById("NewestCfpContent");
            
            return new CfpSingleItemPage(Driver, webElement);
        }

        public CfpSingleItemPage ShowsMostViewedCfp()
        {
            var webElement = Driver.FindElementById("MostViewedCfpContent");

            return new CfpSingleItemPage(Driver, webElement);
        }

        public CfpSingleItemPage ShowsRandomCfp()
        {
            var webElement = Driver.FindElementById("RandomCfpContent");

            return new CfpSingleItemPage(Driver, webElement);
        }
    }
}
