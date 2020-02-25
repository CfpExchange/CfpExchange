using CfpExchange.FunctionalTests.PageObjectModels;
using Xunit;

namespace CfpExchange.FunctionalTests.Tests
{
    public class HomePageTest : TestBase
    {
        [Fact]
        public void HomePage_ShouldShowNewestCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.True(homePage.ShowsNewestCfp()
                .HasClickableTitle());
        }

        [Fact]
        public void HomePage_ShouldShowMostViewedCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.True(homePage.ShowsMostViewedCfp()
                .HasClickableTitle());
        }

        [Fact]
        public void HomePage_ShouldShowRandomCfp()
        {
            var homePage = new HomePage(Driver);

            Assert.True(homePage.ShowsRandomCfp()
                .HasClickableTitle());
        }
    }
}
