using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class SharedPageActions : PageFunctions
    {
        public SharedPageActions(RemoteWebDriver driver) : base(driver)
        {
        }

        //public HomePage NavigateHome()
        //{
        //}

        //public CfpNewestPage NavigateNewestCfp()
        //{
        //}

        //public AboutPage NagitageToAbout()
        //{
        //}

        public CfpSubmitPage NavigateToNewCfp()
        {
            Driver.FindElement(By.Id("submitCfpButton")).Click();

            return new CfpSubmitPage(Driver);
        }
    }
}
