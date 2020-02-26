using System;

namespace CfpExchange.Functions
{
    public abstract class BaseFunction
    {
        protected static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
