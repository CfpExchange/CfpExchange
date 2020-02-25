using CfpExchange.FunctionalTests.PageObjectModels;
using Xunit;

namespace CfpExchange.FunctionalTests.Tests
{
    public class CfpSubmitPageTest : TestBase
    {
        [Fact]
        public void CfpSubmitPage()
        {
            var cfpSubmitPage = new CfpSubmitPage(Driver);

            Assert.True(cfpSubmitPage.NavigateToNewCfp()
                .OnCfpSubmitPage());
        }
    }
}
