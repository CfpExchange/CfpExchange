using CfpExchange.FunctionalTests.PageObjectModels;
using NUnit.Framework;

namespace CfpExchange.FunctionalTests.Tests
{
    [TestFixture]
    public class CfpSubmitPageTest : TestBase
    {
        [Test]
        public void CfpSubmitPage()
        {
            var cfpSubmitPage = new CfpSubmitPage(Driver);

            Assert.IsTrue(cfpSubmitPage.NavigateToNewCfp()
                .OnCfpSubmitPage());
        }
    }
}
