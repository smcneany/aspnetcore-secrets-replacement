using AspnetCoreSecretsReplacement.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspnetCoreSecretsReplacement.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //NOTE: This a console application, not a web application. Your Program.cs file will look different
            //The remainder of the sample (Startup.cs, appsettings.json) will be the same in Console or Web projects.

            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            
            // Get Service if needed
            var service = serviceProvider.GetService<ISampleService>();
        }
    }
}
