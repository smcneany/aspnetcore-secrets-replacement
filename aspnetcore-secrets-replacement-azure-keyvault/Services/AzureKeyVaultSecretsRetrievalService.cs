using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AspnetCoreSecretsReplacement.AzureKeyVault.Configuration;
using AspnetCoreSecretsReplacement.Core.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspnetCoreSecretsReplacement.AzureKeyVault.Services
{
    public class AzureKeyVaultSecretsRetrievalService : ISecretsRetrievalService
    {
        public readonly AzureKeyVaultConfigurationSettings _azureKeyVaultConfigurationSettings;

        public AzureKeyVaultSecretsRetrievalService(
            IOptions<AzureKeyVaultConfigurationSettings> azureKeyVaultConfigurationSettings)
        {
            _azureKeyVaultConfigurationSettings = azureKeyVaultConfigurationSettings.Value;
        }

        public async Task<string> GetAsync(string key)
        {
            TokenCredential tokenCredential;
            if (_azureKeyVaultConfigurationSettings.UseManagedIdentity)
            {
                tokenCredential = new ManagedIdentityCredential();
            }
            else
            {
                tokenCredential = new ClientSecretCredential(
                    _azureKeyVaultConfigurationSettings.TenantId,
                    _azureKeyVaultConfigurationSettings.ClientId,
                    _azureKeyVaultConfigurationSettings.ClientSecret);
            }
            var client = new SecretClient(new Uri(_azureKeyVaultConfigurationSettings.VaultUri), tokenCredential);

            //Key not found in local dictionary. Get it from Azure Key Vault and put it in local secrets store for later use
            var secret = await client.GetSecretAsync(key);
            if (secret == null || secret.Value == null || secret.Value.Value == null)            
            {
                throw new ArgumentException($"The key {key} was not found in Azure Key Vault.");
            }

            return secret.Value.Value;
        }
    }
}
