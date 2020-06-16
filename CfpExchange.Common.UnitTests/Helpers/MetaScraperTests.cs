using System;

using CfpExchange.Common.Helpers;

using Xunit;

namespace CfpExchange.Common.UnitTests.Helpers
{
    public class MetaScraperTests
    {
        #region Constants

        private const string INVALID_URL = "no-valid";
        private const string VALID_URL = "https://github.com";
        private const string VALID_URL_TITLE = "Build software better, together";

        #endregion

        [Fact]
        public void GetMetaDataFromUrl_WithUrlNull_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => MetaScraper.GetMetaDataFromUrl(null));

            Assert.Equal("url", exception.ParamName);
        }

        [Fact]
        public void GetMetaDataFromUrl_WithUrlInvalid_ReturnsError()
        {
            var result = MetaScraper.GetMetaDataFromUrl(INVALID_URL);

            Assert.NotNull(result);
            Assert.True(result.HasError);
            Assert.False(result.ExternalPageError);
        }

        [Fact]
        // TODO: Fix. Not happy with actually calling out to (and having a dependency on) an external website,
        // but loading from file is a different call for Html Agility Pack. Might need a wrapper to enable DI.
        public void GetMetaDataFromUrl_WithUrlValid_ReturResult()
        {
            var result = MetaScraper.GetMetaDataFromUrl(VALID_URL);

            Assert.NotNull(result);
            Assert.Equal(VALID_URL_TITLE, result.Title);
        }
    }
}
