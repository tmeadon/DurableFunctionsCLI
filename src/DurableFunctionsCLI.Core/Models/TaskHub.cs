using Azure.Data.Tables;

namespace DurableFunctionsCLI.Core.Models
{
    public class TaskHub
    {
        public string Name { get; set; }
        public string StorageAccountName { get; set; }
        public TableClient HistoryTableClient { get; set; }
        public TableClient InstancesTableClient { get; set; }
    }
}