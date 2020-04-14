using OpenQA.Selenium;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class HomePage : BasePage
    {
        #region Constructors

        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        #endregion

        public CfpSingleItemPage ShowsNewestCfp()
        {
            var webElement = _driver.FindElement(By.Id("NewestCfpContent"));
            
            return new CfpSingleItemPage(_driver, webElement);
        }

        public CfpSingleItemPage ShowsMostViewedCfp()
        {
            var webElement = _driver.FindElement(By.Id("MostViewedCfpContent"));

            return new CfpSingleItemPage(_driver, webElement);
        }

        public CfpSingleItemPage ShowsRandomCfp()
        {
            var webElement = _driver.FindElement(By.Id("RandomCfpContent"));

            return new CfpSingleItemPage(_driver, webElement);
        }
    }
}
