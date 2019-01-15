using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace CfpExchange.FunctionalTests.Tests
{
    public class TestBase
    {
        protected ChromeDriver Driver;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Driver = Drivers.GetChromeDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Drivers.DisposeChomeDriver();
        }
    }
}
