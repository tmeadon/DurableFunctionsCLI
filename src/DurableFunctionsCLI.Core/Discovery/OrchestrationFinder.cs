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

        public async Task<IEnumerable<Orchestration>> GetOrchestrationsAsync(DateTime sinceDate)
        {
            return await QueryInstanceTableAsync(o => o.CreatedTime > sinceDate);
        }

        public async Task<IEnumerable<Orchestration>> GetOrchestrationsAsync(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("End date should not be before the start date");

            return await QueryInstanceTableAsync(o => o.CreatedTime > start && o.CreatedTime < end);
        }

        private async Task<IEnumerable<Orchestration>> QueryInstanceTableAsync(Expression<Func<Orchestration,bool>> filter)
        {
            AsyncPageable<Orchestration> result = taskHub.InstancesTableClient.QueryAsync<Orchestration>(filter);
            var orchestrations = await BuildOrchestrationList(result);
            return orchestrations;
        }

        private async Task<IEnumerable<Orchestration>> BuildOrchestrationList(AsyncPageable<Orchestration> orchestrationsPageable)
        {
            List<Orchestration> orchestrations = new List<Orchestration>();
            
            await foreach (var orchestration in orchestrationsPageable)
            {
                orchestrations.Add(orchestration);
            }

            return orchestrations;
        }
    }
}