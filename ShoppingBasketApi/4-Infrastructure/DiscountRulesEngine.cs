using System.Text.Json;
using RulesEngine.Models;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Infrastructure
{
    public class DiscountRulesEngine : IRulesEngine
    {
        private readonly IRulesFileProvider rulesFileProvider;
        private readonly IEnumerable<RulesEngine.Models.Workflow> workflows;

        public DiscountRulesEngine(IRulesFileProvider rulesFileProvider)
        {
            this.rulesFileProvider = rulesFileProvider;
            var jsonContent = this.rulesFileProvider.GetRulesJsonAsync().Result;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            this.workflows = JsonSerializer.Deserialize<List<RulesEngine.Models.Workflow>>(jsonContent, options);
        }

        public async Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, BasketInput[] inputs)
        {
            var workflowsToExecute = this.workflows
                .Where(w => w.WorkflowName == workflowName)
                .ToList();

            if (!workflowsToExecute.Any())
            {
                return default;
            }

            var rulesEngine = new RulesEngine.RulesEngine(workflowsToExecute.ToArray(), null);

            return await rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);
        }

        public async Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, BasketItemInput[] inputs)
        {
            var workflowsToExecute = this.workflows
                .Where(w => w.WorkflowName == workflowName)
                .ToList();

            if (!workflowsToExecute.Any())
            {
                return default;
            }

            var rulesEngine = new RulesEngine.RulesEngine(workflowsToExecute.ToArray(), null);

            return await rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);
        }
    }
}

