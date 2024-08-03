using System;
using Rule = ShoppingBasketApi.Domain.Entities.Rule;
using Workflow = ShoppingBasketApi.Domain.Entities.Workflow;
namespace ShoppingBasketApi.Data.Repositories
{
    public interface IRulesRepository
    {
        Task<IEnumerable<Workflow>> GetAllWorkflowsAsync();
        Task<Workflow> GetWorkflowByIdAsync(int id);
        Task AddWorkflowAsync(Workflow workflow);
        Task UpdateWorkflowAsync(Workflow workflow);
        Task DeleteWorkflowAsync(int id);
        Task<IEnumerable<Rule>> GetAllRulesAsync();
        Task<Rule> GetRuleByIdAsync(int id);
        Task AddRuleAsync(Rule rule);
        Task UpdateRuleAsync(Rule rule);
        Task DeleteRuleAsync(int id);
    }
}

