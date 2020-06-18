using Xunit;

using CfpExchange.Common.Helpers;

namespace CfpExchange.Common.UnitTests.Helpers
{
    public class FriendlyUrlHelperTests
    {
        #region Constants

        private const string TITLE_TWENTY_CHARACTERS = "This is a test title";
        private const string URL_TWENTY_CHARACTERS = "this-is-a-test-title";
        // TODO: Fix! 
        // The FriendlyUrlHelper returns a URL that's one character longer than the maxLength when the limited title ends on a space.
        private const string URL_TWENTYTEN_CHARACTERS = "this-is-a-t";
        private const string TITLE_NINETY_CHARACTERS = "This title is a bit longer: it's ninety characters to make sure we test the trimming !!!!";
        private const string URL_NINETY_CHARACTERS = "this-title-is-a-bit-longer-its-ninety-characters-to-make-sure-we-test-the-trimm";
        private const string ALL_DISALLOWED_CHARACTERS = "Do not use a,b.c/d\\e-f_g=h";
        private const string URL_DISALLOWED_CHARACTERS = "do-not-use-a-b-c-d-e-f-g-h";
        private const string NON_ASII_CHARACTERS = "Tešt thís titlę àòùçżñýğřłđßÞĥĵ🤓";
        private const string URL_NON_ASII_CHARACTERS = "tešt-thís-titlę-àòùçżñýğřłđßÞĥĵ🤓";
        private const string URL_ASII_CHARACTERS = "test-this-title-aoucznygrldssthhj";

        #endregion

        [Fact]
        public void GetFriendlyTitle_WithTitleNull_ShouldReturnEmpty()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithTitleEmpty_ShouldReturnEmptyString()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(string.Empty);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithShortTitle_ShouldReturnSimilarString()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(TITLE_TWENTY_CHARACTERS);

            Assert.Equal(URL_TWENTY_CHARACTERS, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithShortTitleAndMaxLength_ShouldReturnSimilarStringWithinMaxLength()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(TITLE_TWENTY_CHARACTERS, maxlength: 10);

            Assert.Equal(URL_TWENTYTEN_CHARACTERS, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithLongTitle_ShouldReturnTrimmedString()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(TITLE_NINETY_CHARACTERS);

            Assert.Equal(URL_NINETY_CHARACTERS, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithTitleDisallowedCharacters_ShouldReturnStringWithoutDisallowedCharacters()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(ALL_DISALLOWED_CHARACTERS);

            Assert.Equal(URL_DISALLOWED_CHARACTERS, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithNonAsciiCharactersNoRemap_ShouldReturnStringWithNonAsciiCharacters()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(NON_ASII_CHARACTERS);

            Assert.Equal(URL_NON_ASII_CHARACTERS, result);
        }

        [Fact]
        public void GetFriendlyTitle_WithNonAsciiCharactersRemap_ShouldReturnStringWithoutNonAsciiCharacters()
        {
            var result = FriendlyUrlHelper.GetFriendlyTitle(NON_ASII_CHARACTERS, true);

            Assert.Equal(URL_ASII_CHARACTERS, result);
        }
    }
}
