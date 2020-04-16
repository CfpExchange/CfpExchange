using System;

namespace CfpExchange.Common.Services
{
    public abstract class BaseService
    {
        protected static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

    }
}
