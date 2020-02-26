using OpenQA.Selenium;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class BasePage
    {
        #region Fields

        protected readonly IWebDriver _driver;

        #endregion

        #region Constructors

        public BasePage(IWebDriver driver)
        {
            _driver = driver;
        }

        #endregion

        protected bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
