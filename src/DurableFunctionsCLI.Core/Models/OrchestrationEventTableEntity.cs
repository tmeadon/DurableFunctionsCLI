using Azure;
using Azure.Data.Tables;
using System;

namespace DurableFunctionsCLI.Core.Models
{
    public class OrchestrationEventTableEntity : OrchestrationEvent, ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}