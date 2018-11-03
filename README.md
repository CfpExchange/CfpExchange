# ![CFP Exchange logo](https://cfp.exchange/images/exchange50x50.png) CFP.Exchange [![Build status](https://jfversluis.visualstudio.com/CfpExchange/_apis/build/status/CfpExchange%20CI)](https://jfversluis.visualstudio.com/CfpExchange/_build/latest?definitionId=30)

An online community website used to share interesting call for papers with
speakers. 

This website is built using ASP.NET Core 2.0 and hosted on Azure: [https://cfp.exchange](https://cfp.exchange).

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
    "TwitterConsumerKey": "key",
    "TwitterConsumerSecret": "secret",
    "TwitterOAuthToken": "token",
    "TwitterOAuthTokenSecret": "tokensecret",
    "AdminEmailaddress": "you@yourdomain.com",
    "EmailSettings": {
        "ApiKey": "MailGunApiKey",
        "ApiUri": "https://api.mailgun.net/v3/yourdomain.com/messages",
        "From": "No-Reply CFP Exchange <no-reply@cfp.exchange>"
    },
    "MapsApiKey": "key",
    "ConnectionStrings": {
        "CfpExchangeDb":  "ConnectionStringToTheCfpExchangeDatabase" 
    },
    "UrlPreviewApiKey": "key",
    "FeatureToggle": {
        "HostOwnImages": true
    },
    "ServicebusEventImagesQueueConnectionString": "ConnectionStringToTheServiceBusQueue"
}
```

- `MapsApiKey` is an API key for the Azure Maps service
- `UrlPreviewApiKey` is the Azure Cognitive Services URL Preview key (experimental) functionality for this is implemented but not currently used.

### Azure Functions settings
In order to run the Azure Functions project, you need to create a file called `local.settings.json` in the root directory of this project.

The contents should look like the following:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "ServicebusQueueConnectionString": "ConnectionStringToTheServiceBusQueue",
    "StorageAccountName": "ConnectionStringToTheBlobStorageAccount",
    "CfpExchangeDb": "ConnectionStringToTheCfpExchangeDatabase"
  }
}
```

All of these settings can be found within the Azure Portal once your environment is deployed to Azure via the ARM template.
