using System;

namespace CfpExchange.Functions
{
    public abstract class BaseFunction
    {
        protected static string GetEnvironmentVariable(string settingName)
        {
            return Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process);
        }
    }
}
