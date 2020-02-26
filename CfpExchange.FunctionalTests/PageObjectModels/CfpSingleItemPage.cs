using OpenQA.Selenium;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class CfpSingleItemPage : BasePage
    {
        #region Fields

        private readonly IWebElement _webElement;

        #endregion

        #region Constructors

        public CfpSingleItemPage(IWebDriver driver, IWebElement webElement) : base(driver)
        {
            _webElement = webElement;
        }

        #endregion

        public bool HasClickableTitle()
        {
            var title = _webElement.FindElement(By.TagName("h3"));

            var titleLink = title.FindElement(By.TagName("a"));

            if (!string.IsNullOrWhiteSpace(titleLink.Text))
            {
                return true;
            }

            return false;
        }
    }
}
