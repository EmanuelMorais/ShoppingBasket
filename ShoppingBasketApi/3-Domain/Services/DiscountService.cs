using RulesEngine.Models;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Services;

public class DiscountService : IDiscountService
{
    private readonly IRulesEngine _rulesEngine;

    public DiscountService(IRulesEngine rulesEngine)
    {
        _rulesEngine = rulesEngine;
    }

    public async Task<Result<Basket>> ApplyBasketDiscountAsync(Basket basket)
    {
        try
        {
            var input = basket.Items.Select(item => new
            {
                ProductName = item.ItemName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToArray();

            var ruleResults = await _rulesEngine.ExecuteAllRulesAsync("Discounts", new[] { input });

            foreach (var result in ruleResults)
            {
                if (result.IsSuccess)
                {
                    //this should be a rule action in a more robust version
                    var discount = Convert.ToDecimal(result.Rule.SuccessEvent);
                    if (result.Rule.RuleName == "MultiBuySoupBread")
                    {
                        var breadItem = new BasketItem { UnitPrice = 2, Quantity = 1, DiscountApplied = discount * 100 };
                        basket.Items.Add(breadItem);
                    }
                }
            }

            return basket;
        }
        catch (Exception)
        {
            return Result<Basket>.Failure(ErrorCode.GenericError, ErrorMessages.InvalidRequest);
        }
    }

    public async Task<Result<Receipt>> ApplyDiscountsAsync(Basket basket)
    {
        try
        {
            var inputs = basket.GetInputs();
            var ruleResults = await _rulesEngine.ExecuteAllRulesAsync("Discounts", inputs);

            var totalDiscountPercentage = ruleResults
                .Where(result => result.IsSuccess)
                .Sum(result => GetDiscountFromActionResult(result)) * 100;

            var totalPriceBeforeDiscounts = basket.Items.Sum(item => item.Price);

            decimal totalDiscountAmount = totalPriceBeforeDiscounts * (totalDiscountPercentage / 100);

            var receipt = new Receipt
            {
                DiscountsApplied = totalDiscountPercentage,
                TotalPrice = totalPriceBeforeDiscounts - totalDiscountAmount
            };

            return Result<Receipt>.Success(receipt);
        }
        catch (Exception)
        {
            return Result<Receipt>.Failure(ErrorCode.GenericError, ErrorMessages.InvalidRequest);
        }
    }

    private decimal GetDiscountFromActionResult(RuleResultTree ruleResult)
    {
        if (ruleResult.IsSuccess && ruleResult.Rule != null)
        {
            var successEvent = ruleResult.Rule.SuccessEvent;
            if (decimal.TryParse(successEvent, out var discount))
            {
                return discount;
            }
        }

        return 0m;
    }

}


