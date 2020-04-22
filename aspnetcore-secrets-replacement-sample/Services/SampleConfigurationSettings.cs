using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreSecretsReplacement.Sample.Services
{
    public class SampleConfigurationSettings
    {
        public string AccountName { get; set; } //Non-sensitive property
        public string AccountKey { get; set; } //Sensitive property
    }
}
