using OpenQA.Selenium;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class CfpSubmitPage : BasePage
    {
        #region Constructors

        public CfpSubmitPage(IWebDriver driver) : base(driver)
        {
        }

        #endregion

        public bool OnCfpSubmitPage()
        {
            return IsElementPresent(By.Id("SubmitCfpTitle"));
        }

        public CfpSubmitPage NavigateToNewCfp()
        {
            _driver.FindElement(By.Id("submitCfpButton")).Click();

            return new CfpSubmitPage(_driver);
        }

    }
}
