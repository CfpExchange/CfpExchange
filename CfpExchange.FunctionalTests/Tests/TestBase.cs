using OpenQA.Selenium.Chrome;

namespace CfpExchange.FunctionalTests.Tests
{
    public class TestBase
    {
        protected static ChromeDriver Driver = Drivers.GetChromeDriver();
    }
}
