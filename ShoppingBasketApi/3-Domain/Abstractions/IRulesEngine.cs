using RulesEngine.Models;
using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Domain.Abstractions;

public interface IRulesEngine
{
    Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, BasketInput[] inputs);
    Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, BasketItemInput[] inputs);
}

