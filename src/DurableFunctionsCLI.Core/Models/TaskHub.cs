namespace DurableFunctionsCLI.Core.Models
{
    public class TaskHub
    {
        public string Name { get; set; }
        public string StorageAccountName { get; set; }
        public string HistoryTableName { get; set; }
        public string InstancesTableName { get; set; }
    }
}