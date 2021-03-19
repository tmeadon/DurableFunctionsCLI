using System;

namespace DurableFunctionsCLI.Core.Models
{
    public class OrchestrationEvent
    {
        public string Name { get; set; }
        public string Result { get; set; }
        public DateTime _Timestamp { get; set; }
        public string ExecutionId { get; set; }
        public string Input { get; set; }
        public int? EventId { get; set; }
        public int? TaskScheduledId { get; set; }
        public string OrchestrationStatus { get; set; }
    }
}