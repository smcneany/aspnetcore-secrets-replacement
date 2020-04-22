using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspnetCoreSecretsReplacement.Core.Services
{
    public interface ISecretsRetrievalService
    {
        Task<string> GetAsync(string key);
    }
}
