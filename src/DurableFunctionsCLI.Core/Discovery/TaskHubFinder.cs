using DurableFunctionsCLI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DurableFunctionsCLI.Core.Discovery
{
    public interface ITaskHubFinder
    {
        Task<IEnumerable<TaskHub>> FindAllAsync();
    }

    public class TaskHubFinder : ITaskHubFinder
    {
        private StorageAccountFinder storageAccountFinder;
        private ITaskHubDetector taskHubDetector;

        internal TaskHubFinder(StorageAccountFinder storageAccountFinder, ITaskHubDetector taskHubDetector)
        {
            this.storageAccountFinder = storageAccountFinder;
            this.taskHubDetector = taskHubDetector;
        }

        public async Task<IEnumerable<TaskHub>> FindAllAsync()
        {
            var storageAccounts = await storageAccountFinder.FindAllStorageAccountsAsync();
            return await FindTaskHubsInStorageAccountsAsync(storageAccounts);
        }

        private async Task<IEnumerable<TaskHub>> FindTaskHubsInStorageAccountsAsync(IEnumerable<StorageAccount> storageAccounts)
        {
            List<TaskHub> taskHubs = new List<TaskHub>();

            foreach (var storageAccount in storageAccounts)
            {
                await FindTaskHubsIfStorageAccountValidAsync(taskHubs, storageAccount);
            }

            return taskHubs;
        }

        private async Task FindTaskHubsIfStorageAccountValidAsync(List<TaskHub> taskHubs, StorageAccount storageAccount)
        {
            if (IsValidTableStorageAccount(storageAccount))
            {
                var detectedHubs = await taskHubDetector.DetectAsync(storageAccount);
                taskHubs.AddRange(detectedHubs);
            }
        }

        private bool IsValidTableStorageAccount(StorageAccount storageAccount)
        {
            var hasTableEndpoint = storageAccount.Properties.PrimaryEndpoints.ContainsKey("table");
            var isCorrectKind = storageAccount.Kind == "StorageV2" || storageAccount.Kind == "Storage";
            return hasTableEndpoint && isCorrectKind;
        }
    }
}