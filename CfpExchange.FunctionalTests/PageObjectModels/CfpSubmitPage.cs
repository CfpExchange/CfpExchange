using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class CfpSubmitPage : SharedPageActions
    {
        public CfpSubmitPage(RemoteWebDriver driver) : base(driver)
        {
        }

        public bool OnCfpSubmitPage()
        {
            return IsElementPresent(By.Id("SubmitCfpTitle"));
        }
    }
}
