namespace DurableFunctionsCLI.Core.Discovery
{
    public class TaskHubFinderFactory
    {
        private string subscriptionId;
        private string bearerToken;
        private string resourceGroupName;

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

            return new ResourceGroupStorageAccountFinder(subscriptionId, resourceGroupName, bearerToken);
        }
    }
}