using System;
using System.IO;
using System.Reflection;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CfpExchange.FunctionalTests.Tests
{
    public abstract class TestBase : IDisposable
    {
        #region Fields

        protected readonly ChromeDriver _chromeDriver;

        #endregion

        #region Constructors

        public TestBase()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--window-size=1300,1000");

            _chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);
            _chromeDriver.Navigate().GoToUrl(@"http://localhost:55556/");
        }

        #endregion

        protected bool IsElementPresent(By by)
        {
            try
            {
                _ = _chromeDriver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _chromeDriver.Quit();
            _chromeDriver.Dispose();
        }
    }
}
