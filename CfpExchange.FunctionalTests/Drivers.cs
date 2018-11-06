using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chrome;

namespace CfpExchange.FunctionalTests
{
    public static class Drivers
    {
        private static ChromeDriver _chromeDriver;

        public static ChromeDriver GetChromeDriver()
        {
            if (_chromeDriver == null)
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--window-size=1300,1000");

                _chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    chromeOptions);
            }

            _chromeDriver.Navigate().GoToUrl(@"http://localhost:55556/");

            return _chromeDriver;
        }

        public static void DisposeChomeDriver()
        {
            _chromeDriver.Dispose();
            _chromeDriver = null;
        }
    }
}
