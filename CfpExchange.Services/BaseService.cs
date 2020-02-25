using System;

namespace CfpExchange.Services
{
    public abstract class BaseService
    {
        protected static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

    }
}
