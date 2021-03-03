using DurableFunctionsCLI.Core.Models;
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
            return GetTaskHubsFromTableNames(allTables, storageAccount);
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

        private IEnumerable<TaskHub> GetTaskHubsFromTableNames(IEnumerable<string> allTableNames, StorageAccount storageAccount)
        {
            var potentialTaskHubNames = allTableNames.Where(n => n.EndsWith(instanceTableSuffix)).Select(n => n.Replace(instanceTableSuffix, string.Empty));
            var validTaskHubs = potentialTaskHubNames.Where(n => IsValidTaskHub(allTableNames, n));
            return BuildTaskHubList(validTaskHubs, storageAccount);
        }

        private bool IsValidTaskHub(IEnumerable<string> allTableNames, string potentialTaskHubName)
        {
            var instanceTableName = $"{potentialTaskHubName}{instanceTableSuffix}";
            var historyTableName = $"{potentialTaskHubName}{historyTableSuffix}";
            return allTableNames.Contains(instanceTableName) && allTableNames.Contains(historyTableName);
        }

        private IEnumerable<TaskHub> BuildTaskHubList(IEnumerable<string> validTaskHubs, StorageAccount storageAccount)
        {
            foreach (var taskHub in validTaskHubs)
            {
                yield return new TaskHub
                {
                    Name = taskHub,
                    StorageAccountName = storageAccount.Name,
                    HistoryTableName = $"{taskHub}{historyTableSuffix}",
                    InstancesTableName = $"{taskHub}{instanceTableSuffix}"
                };
            }
        }
    }
}