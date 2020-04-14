using System;

namespace CfpExchange.Common.Helpers
{
    public static class SettingsHelper
    {
        public static string GetEnvironmentVariable(string settingName)
        {
            Guard.IsNotNull(settingName, nameof(settingName));

            return Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process);
        }
    }
}
