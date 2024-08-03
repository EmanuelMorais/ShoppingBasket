using System.Text.Json;
using RulesEngine.Models;
using ShoppingBasketApi.Domain.Abstractions;

namespace ShoppingBasketApi.Infrastructure
{
    public class DiscountRulesEngine : IRulesEngine
    {
        public async Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, dynamic[] inputs)
        {
            string jsonContent = await GetJsonContent();

            var workflows = JsonSerializer.Deserialize<List<Domain.Entities.Workflow>>(jsonContent);

            var workflowsToExecute = workflows
                .Where(w => w.Name == workflowName)
                .Select(w => new RulesEngine.Models.Workflow
                {
                    WorkflowName = w.Name,
                    Rules = w.Rules.Select(r => new RulesEngine.Models.Rule
                    {
                        RuleName = r.RuleName,
                        SuccessEvent = r.SuccessEvent,
                        ErrorMessage = r.ErrorMessage,
                        Expression = r.Expression
                    }).ToList()
                }).ToList();

            if (!workflowsToExecute.Any())
            {
                throw new ArgumentException($"No configuration found for workflow: {workflowName}");
            }

            var rulesEngine = new RulesEngine.RulesEngine(workflowsToExecute.ToArray(), null);

            return await rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);
        }

        private static async Task<string> GetJsonContent()
        {
            string baseDirectory = AppContext.BaseDirectory;
            string relativePath = Path.Combine("..", "..", "..", "4-Infrastructure", "rules.json");
            string filePath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Rules file not found at path: {filePath}");
            }

            string jsonContent = await File.ReadAllTextAsync(filePath);

            return jsonContent;
        }
    }
}

