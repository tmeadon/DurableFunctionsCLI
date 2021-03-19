
using System;

namespace DurableFunctionsCLI.Core.Models
{
    public class Orchestration
    {
        public string Name { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string RuntimeStatus { get; set; }
        public string TaskHubName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public string ExecutionId { get; set; }
    }
}