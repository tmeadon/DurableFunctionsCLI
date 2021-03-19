using Azure;
using DurableFunctionsCLI.Core.Helpers;
using DurableFunctionsCLI.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace DurableFunctionsCLI.Core.Discovery
{
    public class OrchestrationExpander
    {
        private TaskHub taskHub;
        private string executionId;

        public OrchestrationExpander(TaskHub taskHub, string executionId)
        {
            this.taskHub = taskHub;
            this.executionId = executionId;
        }

        public IEnumerable<OrchestrationEvent> GetOrchestrationEvents()
        {
            var allEvents = QueryAllOrchestrationEvents();
            var collapsedEvents = CollapseEventList(allEvents);
            return collapsedEvents;
        }

        private IEnumerable<OrchestrationEvent> QueryAllOrchestrationEvents()
        {
            Pageable<OrchestrationEventTableEntity> result = taskHub.HistoryTableClient.Query<OrchestrationEventTableEntity>(h => h.ExecutionId == executionId);
            var eventList = BuildEventList(result);
            return eventList;
        }

        private IEnumerable<OrchestrationEvent> BuildEventList(Pageable<OrchestrationEventTableEntity> tableEntities)
        {
            List<OrchestrationEvent> events = new List<OrchestrationEvent>();

            foreach (var item in tableEntities)
            {
                events.Add(TableEntityConverter.ConvertToBaseType<OrchestrationEvent>(item));
            }

            return events;
        }

        private IEnumerable<OrchestrationEvent> CollapseEventList(IEnumerable<OrchestrationEvent> allEvents)
        {
            var collapsedEventList = allEvents.Where(e => e.EventId != -1 && e.EventId != null);

            foreach (var item in collapsedEventList)
            {
                CollapseEvent(item, allEvents);
            }

            return collapsedEventList;
        }

        private void CollapseEvent(OrchestrationEvent thisEvent, IEnumerable<OrchestrationEvent> allEvents)
        {
            var resultEvent = allEvents.Where(e => e.TaskScheduledId == thisEvent.EventId).FirstOrDefault();
            SetEventResultProperties(thisEvent, resultEvent);
        }

        private void SetEventResultProperties(OrchestrationEvent thisEvent, OrchestrationEvent resultEvent)
        {
            if (resultEvent != null)
                thisEvent.Result = resultEvent.Result;

            if (string.IsNullOrEmpty(thisEvent.Name))
                thisEvent.Name = thisEvent.OrchestrationStatus;
        }
    }
}