using AspnetCoreSecretsReplacement.AzureKeyVault.Services;
using AspnetCoreSecretsReplacement.Core.Services;
using AspnetCoreSecretsReplacement.Sample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspnetCoreSecretsReplacement.Sample
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigurationRoot>(Configuration);

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
        }
    }
}
