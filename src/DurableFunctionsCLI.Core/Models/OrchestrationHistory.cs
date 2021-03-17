using System;

namespace DurableFunctionsCLI.Core.Models
{
    public class OrchestrationHistory
    {
        public string Name { get; set; }
        public string Result { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public Guid ExecutionId { get; set; }
    }
}