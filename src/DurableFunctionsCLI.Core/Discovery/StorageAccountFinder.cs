using DurableFunctionsCLI.Core.Exceptions;
using DurableFunctionsCLI.Core.Models;
using System;   
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DurableFunctionsCLI.Core.Discovery
{
    internal abstract class StorageAccountFinder
    {
        protected string subscriptionId;
        protected string bearerToken;

        public StorageAccountFinder(string subscriptionId, string bearerToken)
        {
            this.subscriptionId = subscriptionId;
            this.bearerToken = bearerToken;
        }

        public abstract Task<IEnumerable<StorageAccount>> FindAllStorageAccountsAsync();

        protected virtual async Task<IEnumerable<StorageAccount>> GetStorageAccountsFromAzureAsync(string url)
        {
            try
            {
                return await InvokeWebRequestAsync(url);
            }
            catch (Exception ex)
            {
                HandleGetStorageAccountExceptions(ex);
                throw;
            }
        }

        private async Task<IEnumerable<StorageAccount>> InvokeWebRequestAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var response = await httpClient.GetFromJsonAsync<StorageAccountApiResponse>(url);
                return response.Value;
            }
        }

        private void HandleGetStorageAccountExceptions(Exception ex)
        {
            if (ex.Message.Contains("404"))
                throw new StorageAccountNotFoundException();

            if (ex.Message.Contains("429"))
                throw new StorageApiThrottledException();
        }

        protected class StorageAccountApiResponse
        {
            public List<StorageAccount> Value { get; set; }
        }
    }

    internal class SubscriptionStorageAccountFinder : StorageAccountFinder
    {
        private static readonly string apiUrl = "https://management.azure.com/subscriptions/{0}/providers/Microsoft.Storage/storageAccounts?api-version=2019-06-01";
        private string formattedUrl;

        public SubscriptionStorageAccountFinder(string subscriptionId, string bearerToken) : base(subscriptionId, bearerToken)
        {
            FormatApiUrl();
        }

        private void FormatApiUrl()
        {
            formattedUrl = string.Format(apiUrl, subscriptionId);
        }

        public override async Task<IEnumerable<StorageAccount>> FindAllStorageAccountsAsync()
        {
            return await base.GetStorageAccountsFromAzureAsync(formattedUrl);
        }
    }

    internal class ResourceGroupStorageAccountFinder : StorageAccountFinder
    {
        private static readonly string apiUrl = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Storage/storageAccounts?api-version=2019-06-01";
        private string resourceGroupName;
        private string formattedUrl;

        public ResourceGroupStorageAccountFinder(string subscriptionId, string resourceGroupName, string bearerToken)
            : base(subscriptionId, bearerToken)
        {
            this.resourceGroupName = resourceGroupName;
            FormatApiUrl();
        }

        private void FormatApiUrl()
        {
            formattedUrl = string.Format(apiUrl, subscriptionId, resourceGroupName);
        }

        public override async Task<IEnumerable<StorageAccount>> FindAllStorageAccountsAsync()
        {
            return await base.GetStorageAccountsFromAzureAsync(formattedUrl);
        }
    }

    internal class SpecificStorageAccountFinder : StorageAccountFinder
    {
        private static readonly string apiUrl = "https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Storage/storageAccounts/{2}?api-version=2019-06-01";
        private string resourceGroupName;
        private string storageAccountName;
        private string formattedUrl;

        public SpecificStorageAccountFinder(string subscriptionId, string resourceGroupName, string storageAccountName,
            string bearerToken) : base(subscriptionId, bearerToken)
        {
            this.resourceGroupName = resourceGroupName;
            this.storageAccountName = storageAccountName;
            FormatApiUrl();
        }

        private void FormatApiUrl()
        {
            formattedUrl = string.Format(apiUrl, subscriptionId, resourceGroupName, storageAccountName);
        }

        public override async Task<IEnumerable<StorageAccount>> FindAllStorageAccountsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var response = await httpClient.GetFromJsonAsync<StorageAccount>(formattedUrl);
                return new List<StorageAccount>{ response };
            }  
        }
    }
}