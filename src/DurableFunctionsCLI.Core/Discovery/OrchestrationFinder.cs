using Azure;
using Azure.Data.Tables;
using DurableFunctionsCLI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DurableFunctionsCLI.Core.Discovery
{
    public class OrchestrationFinder
    {
        private TaskHub taskHub;

        public OrchestrationFinder(TaskHub taskHub)
        {
            this.taskHub = taskHub;
        }

        public IEnumerable<Orchestration> GetOrchestrations(DateTime sinceDate)
        {
            return QueryInstanceTable(o => o.CreatedTime > sinceDate);
        }

        public IEnumerable<Orchestration> GetOrchestrations(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("End date should not be before the start date");

            return QueryInstanceTable(o => o.CreatedTime > start && o.CreatedTime < end);
        }

        private IEnumerable<Orchestration> QueryInstanceTable(Expression<Func<OrchestrationTableEntity,bool>> filter)
        {
            Pageable<OrchestrationTableEntity> result = taskHub.InstancesTableClient.Query<OrchestrationTableEntity>(filter);
            var orchestrations = BuildOrchestrationList(result);
            return orchestrations;
        }

        private IEnumerable<Orchestration> BuildOrchestrationList(Pageable<OrchestrationTableEntity> tableEntities)
        {
            List<Orchestration> orchestrations = new List<Orchestration>();
            
            foreach (var orchestration in tableEntities)
            {
                orchestrations.Add(ConvertToOrchestrationBase(orchestration));
            }

            return orchestrations;
        }

        private Orchestration ConvertToOrchestrationBase(OrchestrationTableEntity tableEntity)
        {
            var output = new Orchestration();
            
            foreach (var prop in output.GetType().GetProperties().ToList())
            {
                var value = tableEntity.GetType().GetProperty(prop.Name).GetValue(tableEntity, null);
                prop.SetValue(output, value, null);
            }

            return output;
        }
    }
}