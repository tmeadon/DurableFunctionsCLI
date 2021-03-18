using Azure;
using Azure.Data.Tables;
using System;

namespace DurableFunctionsCLI.Core.Models
{
    internal class OrchestrationTableEntity : Orchestration, ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}