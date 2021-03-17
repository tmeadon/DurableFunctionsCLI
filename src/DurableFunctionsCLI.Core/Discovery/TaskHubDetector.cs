using Azure.Data.Tables;
using DurableFunctionsCLI.Core.Models;
using System;   
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DurableFunctionsCLI.Core.Discovery
{
    internal interface ITaskHubDetector
    {
        Task<IEnumerable<TaskHub>> DetectAsync(StorageAccount storageAccount);
    }

    internal class TaskHubDetector : ITaskHubDetector
    {
        private string bearerToken;
        private string instanceTableSuffix = "Instances";
        private string historyTableSuffix = "History";

        public TaskHubDetector(string bearerToken)
        {
            this.bearerToken = bearerToken;
        }

        public async Task<IEnumerable<TaskHub>> DetectAsync(StorageAccount storageAccount)
        {
            var allTables = await GetStorageAccountTables(storageAccount);
            return await GetTaskHubsFromTableNamesAsync(allTables, storageAccount);
        }

        private async Task<IEnumerable<string>> GetStorageAccountTables(StorageAccount storageAccount)
        {
            var url = GetListTablesApiUrl(storageAccount);
            return await GetAllAccountTablesFromAzureAsync(url);
        }

        private string GetListTablesApiUrl(StorageAccount storageAccount)
        {
            return $"https://management.azure.com{storageAccount.Id}/tableServices/default/tables?api-version=2019-06-01";
        }

        private async Task<IEnumerable<string>> GetAllAccountTablesFromAzureAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var response = await httpClient.GetFromJsonAsync<TablesApiResponse>(url);
                return response.Value.Select(t => t.Name);
            }
        }

        private class TablesApiResponse
        {
            public List<Table> Value { get; set; }
        }

        private class Table
        {
            public string Name { get; set; }
        }

        private async Task<IEnumerable<TaskHub>> GetTaskHubsFromTableNamesAsync(IEnumerable<string> allTableNames, StorageAccount storageAccount)
        {
            var potentialTaskHubNames = allTableNames.Where(n => n.EndsWith(instanceTableSuffix)).Select(n => n.Replace(instanceTableSuffix, string.Empty));
            var validTaskHubs = potentialTaskHubNames.Where(n => IsValidTaskHub(allTableNames, n));
            return await BuildTaskHubListAsync(validTaskHubs, storageAccount);
        }

        private bool IsValidTaskHub(IEnumerable<string> allTableNames, string potentialTaskHubName)
        {
            var instanceTableName = $"{potentialTaskHubName}{instanceTableSuffix}";
            var historyTableName = $"{potentialTaskHubName}{historyTableSuffix}";
            return allTableNames.Contains(instanceTableName) && allTableNames.Contains(historyTableName);
        }

        private async Task<IEnumerable<TaskHub>> BuildTaskHubListAsync(IEnumerable<string> validTaskHubs, StorageAccount storageAccount)
        {
            var taskHubs = new List<TaskHub>();

            foreach (var taskHub in validTaskHubs)
            {
                taskHubs.Add(new TaskHub
                {
                    Name = taskHub,
                    StorageAccountName = storageAccount.Name,
                    HistoryTableClient = await BuildTableClientAsync(storageAccount, $"{taskHub}{historyTableSuffix}"),
                    InstancesTableClient = await BuildTableClientAsync(storageAccount, $"{taskHub}{instanceTableSuffix}")
                });
            }
            
            return taskHubs;
        }

        private async Task<TableClient> BuildTableClientAsync(StorageAccount storageAccount, string tableName)
        {
            var key = await GetStorageAccountKeyAsync(storageAccount);
            return new TableClient(
                new Uri(storageAccount.Properties.PrimaryEndpoints["table"]),
                tableName,
                new TableSharedKeyCredential(storageAccount.Name, key)
            );
        }

        private async Task<string> GetStorageAccountKeyAsync(StorageAccount storageAccount)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var url = GetStorageAccountListKeysUri(storageAccount);
                var response = await httpClient.PostAsync(url, new StringContent(String.Empty));
                var res = await response.Content.ReadAsStringAsync();
                var keys = await response.Content.ReadFromJsonAsync<StorageAccountKeyApiResponse>();
                return keys.Keys.First().Value;
            }
        }

        private string GetStorageAccountListKeysUri(StorageAccount storageAccount)
        {
            return $"https://management.azure.com{storageAccount.Id}/listKeys?api-version=2021-01-01";
        }

        private class StorageAccountKeyApiResponse
        {
            public List<StorageAccountKey> Keys { get; set; }
        }

        private class StorageAccountKey
        {
            public string Value { get; set; }
        }
    }
}