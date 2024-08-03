using System;
using RulesEngine.Models;

namespace ShoppingBasketApi.Domain.Abstractions
{
    public interface IRulesEngine
    {
        Task<List<RuleResultTree>> ExecuteAllRulesAsync(string workflowName, dynamic[] inputs);
    }
}

