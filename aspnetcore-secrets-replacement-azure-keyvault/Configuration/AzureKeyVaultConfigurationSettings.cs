using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreSecretsReplacement.AzureKeyVault.Configuration
{
    public class AzureKeyVaultConfigurationSettings
    {
        public string VaultUri { get; set; }
        public bool UseManagedIdentity { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
