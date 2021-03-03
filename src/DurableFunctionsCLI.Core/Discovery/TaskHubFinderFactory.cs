using System;

namespace DurableFunctionsCLI.Core.Discovery
{
    public class TaskHubFinderFactory
    {
        private string subscriptionId;
        private string bearerToken;
        private string resourceGroupName;
        private string storageAccountName;

        public TaskHubFinderFactory(string subscriptionId, string bearerToken)
        {
            this.subscriptionId = subscriptionId;
            this.bearerToken = bearerToken;
        }

        public TaskHubFinderFactory InResourceGroup(string resourceGroupName)
        {
            this.resourceGroupName = resourceGroupName;
            return this;
        }

        public TaskHubFinderFactory InStorageAccount(string storageAccountName)
        {
            if (resourceGroupName == null)
                throw new InvalidOperationException("Specify a resource group before specifying a specific storage account");

            this.storageAccountName = storageAccountName;
            return this;
        }

        public TaskHubFinder Build()
        {
            var storageFinder = BuildStorageAccountFinder();
            var taskHubDetector = new TaskHubDetector(bearerToken);
            return new TaskHubFinder(storageFinder, taskHubDetector);            
        }

        private StorageAccountFinder BuildStorageAccountFinder()
        {
            if (resourceGroupName == null)
                return new SubscriptionStorageAccountFinder(subscriptionId, bearerToken);

            if (storageAccountName == null)
                return new ResourceGroupStorageAccountFinder(subscriptionId, resourceGroupName, bearerToken);

            return new SpecificStorageAccountFinder(subscriptionId, resourceGroupName, storageAccountName, bearerToken);
        }
    }
}