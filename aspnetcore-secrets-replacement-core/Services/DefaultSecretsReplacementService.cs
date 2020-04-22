using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspnetCoreSecretsReplacement.Core.Services
{
    public class DefaultSecretsReplacementService : ISecretsReplacementService
    {
        private readonly ISecretsRetrievalService _secretsRetrievalService;
        private Dictionary<string, string> _localSecretsStore; //This prevents unnecessary trips to the same Azure Key Vault for the same secret

        public DefaultSecretsReplacementService(
            ISecretsRetrievalService secretsRetrievalService)
        {
            this._secretsRetrievalService = secretsRetrievalService;
            _localSecretsStore = new Dictionary<string, string>();
        }

        public async Task<string> Replace(string originalString)
        {
            if (String.IsNullOrWhiteSpace(originalString))
            {
                return originalString; //Nothing to replace
            }

            var newString = originalString; //New string will eventually have anything in curly braces {key} replaced with the actual value 

            //var keysToReplace = originalString.Split('{', '}');

            Regex regex = new Regex(@"\{\{.*?\}\}");
            var keysToReplace = regex.Matches(originalString);

            foreach (var match in keysToReplace)
            {
                var key = match.ToString().Replace("{","").Replace("}","");

                //Key was found in local dictionary. Use it
                if (_localSecretsStore.ContainsKey(key))
                {
                    newString = newString.Replace($"{{{{{key}}}}}", _localSecretsStore[key]);
                    continue;
                }

                //Key not found in local dictionary. Get it from Azure Key Vault and put it in local secrets store for later use
                var secretValue = await _secretsRetrievalService.GetAsync(key);
                if (!string.IsNullOrWhiteSpace(secretValue))
                {
                    newString = newString.Replace($"{{{{{key}}}}}", secretValue);
                    _localSecretsStore.Add(key, secretValue);
                }
            }

            return newString;
        }
    }
}
