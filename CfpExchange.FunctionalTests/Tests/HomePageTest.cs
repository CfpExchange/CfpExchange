using CfpExchange.FunctionalTests.PageObjectModels;
using NUnit.Framework;

namespace CfpExchange.FunctionalTests.Tests
{
    [TestFixture]
    public class HomePageTest : TestBase
    {
        [Test]
        public void HomePage_ShouldShowNewestCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.IsTrue(homePage.ShowsNewestCfp()
                .HasClickableTitle());
        }

        [Test]
        public void HomePage_ShouldShowMostViewedCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.IsTrue(homePage.ShowsMostViewedCfp()
                .HasClickableTitle());
        }

        [Test]
        public void HomePage_ShouldShowRandomCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.IsTrue(homePage.ShowsRandomCfp()
                .HasClickableTitle());
        }
    }
}
