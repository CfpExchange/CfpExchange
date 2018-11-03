using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CfpExchange.FunctionalTests.PageObjectModels
{
    public class CfpSingleItemPage : PageFunctions
    {
        private readonly IWebElement _webElement;

        public CfpSingleItemPage(RemoteWebDriver driver, IWebElement webElement) : base(driver)
        {
            _webElement = webElement;
        }

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
