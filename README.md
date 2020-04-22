# aspnetcore-secrets-replacement
Provides a default implementation of a string replacement service that replaces keys with values and an implementation of a key replacement service that abstracts Azure Key Vault.

## Usage

First, install the nuget package at [aspnetcore-secrets-replacement](https://www.nuget.org/packages/aspnetcore-secrets-replacement-azure-keyvault/). Then follow the samples project and copy the following information from the project:

**appsettings.json ** - Register the settings needed for the key vault replacement service and any of your services. The sample project leverages the [IOptions pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1 "IOptions pattern"), which is not absolutely required, but highly recommended as a best practice for configuration management. 

```json
{
  "SampleConfigurationSettings": {
    "AccountName": "MyAccount",
    "AccountKey": "{{KeyVaultKey}}" //This value will be replaced by a call to Azure Key Vault during Startup if configured properly
                                    //Use double-bracket syntax with the key inside to indicate the key should be replaced with the value.
  },
  "AzureKeyVaultConfigurationSettings": {
    "VaultUri": "",                 //Required (example: https://my-key-vault-service.vault.azure.net/)
    "UseManagedIdentity": true,     //Always set to 'true' when deployed to Azure
                                    //IMPORTANT: These settings are not recommended for deployed environments
    "TenantId": "",                 //When 'UseManagedIdentity' is false, this allows a localhost to connect to Key Vault directly
    "ClientId": "",                 //When 'UseManagedIdentity' is false, this allows a localhost to connect to Key Vault directly
    "ClientSecret": ""              //When 'UseManagedIdentity' is false, this allows a localhost to connect to Key Vault directly
  }
}
```

**Startup.cs **- Copy the registration of the ISecretsReplacementService, ISecretsRetrievalService, and get a copy of the secrets replacement services that will be used to replace values in other injected services.

```csharp
//****************************************************
//****************************************************
//Secrets Replacement
//****************************************************
//****************************************************
//Add singleton will ensure Azure Key Vault is called once at startup. 
//When rotating keys, the web app will need to be recycled
services.AddSingleton<ISecretsReplacementService, DefaultSecretsReplacementService>();
services.AddSingleton<ISecretsRetrievalService, AzureKeyVaultSecretsRetrievalService>();

//Get an instance of the replacement service to replace keys in other services.
var secretsReplacementService =
	(ISecretsReplacementService)services
	.BuildServiceProvider()
	.GetService<ISecretsReplacementService>();
```

By getting a copy of the secrets replacement service, you can then use it later in this method to override other settings properties in other services.

```csharp
//****************************************************
//****************************************************
//My Service
//****************************************************
//****************************************************

services.Configure<SampleConfigurationSettings>(options => {
	Configuration.GetSection("SampleConfigurationSettings").Bind(options);

	//Replace key with call to secrets management
	options.AccountKey = secretsReplacementService.Replace(options.AccountKey).Result;
});

services.AddSingleton<ISampleService, SampleService>();
```

Replace the SampleConfigurationSettings, ISampleService, and SampleService with your services. Again, this setup assumes you are using Microsoft'IOptions pattern. You could, however, choose to inject the ISecretsReplacementService directly into your service and perform the replacement there against standard IConfiguration[""] properties. However, having all configuration done in 2 files - appsettings.json and Startup.cs - and then having strong typing for configuration properties is really beneficial. 


## Samples

The project aspnetcore-secrets-replacement-tests contains a sample project that includes registering the key vault service in appsettings.json and Startup.cs, as well as using the key vault replacement service to replace other settings in the appsettings.json and the Startup.cs file. 
