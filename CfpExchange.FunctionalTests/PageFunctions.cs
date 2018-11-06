using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CfpExchange.FunctionalTests
{
    public class PageFunctions
    {
        protected readonly RemoteWebDriver Driver;

        public PageFunctions(RemoteWebDriver driver)
        {
            Driver = driver;
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
