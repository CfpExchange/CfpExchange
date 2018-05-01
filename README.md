# CFP.Exchange
An online community website used to share interesting call for papers with
speakers. 

This website is built using ASP.NET Core 2.0 and hosted on Azure.

## Connection strings and other secrets
This project uses a number of secrets to connect to various APIs and databases.
For this we use the [ASP.NET Core user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.0&tabs=visual-studio).

To enter the connection string for your development database, use the following steps:

### On Windows
Create a file in `%APPDATA%\microsoft\UserSecrets\CfpExchangeSecrets\secrets.json` and paste in the example below - then update it with your own secret values!

### On Mac or Linux
Create a file in `~/.microsoft/usersecrets/CfpExchangeSecrets/secrets.json` and paste in the example below - then update it with your own secret values!

### User Secrets example
The file should have the following layout:
```json
{
    "CfpExchangeDb": "ValidConnectionStringToMSSQLDatabase"
}
```

**//TODO:** _Specify what other settings/secrets we need_