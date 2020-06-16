using System;
using CfpExchange.Common.Helpers;
using Xunit;

namespace CfpExchange.Common.UnitTests.Helpers
{
    public class SettingsHelperTests
    {
        #region Constants

        private const string EXISTING_SETTING = "Some Setting";
        private const string EXISTING_SETTING_VALUE = "Some Value";
        private const string NON_EXISTING_SETTING = "Pretty Sure This Setting Doesn't Exist 🤓";

        #endregion

        #region Constructors

        public SettingsHelperTests()
        {
            Environment.SetEnvironmentVariable(EXISTING_SETTING, EXISTING_SETTING_VALUE);
        }

        #endregion

        [Fact]
        public void GetEnvironmentVariable_WithSettingNameNull_ShouldThrowArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => SettingsHelper.GetEnvironmentVariable(null));

            Assert.Equal("settingName", exception.ParamName);
        }

        [Fact]
        public void GetEnvironmentVariable_WithSettingNameExisting_ShouldReturnSettingValue()
        {
            var settingValue = SettingsHelper.GetEnvironmentVariable(EXISTING_SETTING);

            Assert.Equal(EXISTING_SETTING_VALUE, settingValue);
        }

        [Fact]
        public void GetEnvironmentVariable_WithSettingNameNotExisting_ShouldReturnNull()
        {
            var settingValue = SettingsHelper.GetEnvironmentVariable(NON_EXISTING_SETTING);

            Assert.Null(settingValue);
        }
    }
}
