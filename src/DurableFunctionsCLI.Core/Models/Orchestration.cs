using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;

namespace DurableFunctionsCLI.Core.Models
{
    public class Orchestration : ITableEntity
    {
        public string Name { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string RuntimeStatus { get; set; }
        public string TaskHubName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public Guid ExecutionId { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}