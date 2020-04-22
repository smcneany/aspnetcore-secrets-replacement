using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AspnetCoreSecretsReplacement.Sample.Services
{
    public class SampleService : ISampleService
    {
        private readonly SampleConfigurationSettings _settings;

        public SampleService(IOptions<SampleConfigurationSettings> settings)
        {
            if (settings == null || settings.Value == null)
            {
                throw new ArgumentNullException("Settings were not bound correctly during startup.");
            }

            //Validate any other settings properties here (null checks, ranges, etc.)

            //Now we have a concrete class to use in other methods that was injected during startup
            this._settings = settings.Value;
        }
    }
}
