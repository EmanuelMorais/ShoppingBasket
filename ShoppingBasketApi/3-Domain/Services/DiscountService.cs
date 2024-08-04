using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RulesEngine.Models;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Services;

public class DiscountService : IDiscountService
{
    private readonly IRulesEngine rulesEngine;
    private const string Discounts = "Discounts";

    public DiscountService(IRulesEngine rulesEngine)
    {
        this.rulesEngine = rulesEngine;
    }

    public async Task<Result<Receipt>> ApplyDiscountsAsync(Basket basket)
    {
        try
        {
            var inputs = basket.GetInputs();
            var ruleResults = await this.rulesEngine.ExecuteAllRulesAsync(Discounts, inputs);
            var totalPriceBeforeDiscounts = basket.Items.Sum(item => item.Price);
            decimal totalDiscountPercentage = 0m;

            if (ruleResults != null && ruleResults.Any())
            {
                totalDiscountPercentage = ruleResults
                    .Where(result => result.IsSuccess)
                    .Sum(result => GetDiscountFromActionResult(result)) * 100;
            }

            var totalDiscountAmount = totalDiscountPercentage > 0m ? totalPriceBeforeDiscounts * (totalDiscountPercentage / 100) : 0;

            var receipt = new Receipt
            {
                BasketId = basket.Id,
                DiscountsApplied = totalDiscountAmount,
                TotalPrice = totalPriceBeforeDiscounts - totalDiscountAmount
            };

            return Result<Receipt>.Success(receipt);
        }
        catch (Exception)
        {
            return Result<Receipt>.Failure(ErrorCode.GenericError, ErrorMessages.InvalidRequest);
        }
    }

    public async Task<Result<Basket>> ApplyBasketDiscountAsync(Basket basket)
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            var basketItems = basket.Items.ToList();

            foreach (var basketItem in basketItems)
            {
                var inputs = basketItem.GetInputs(currentDate);
                var ruleResults = await this.rulesEngine.ExecuteAllRulesAsync("Discounts", inputs);

                foreach (var result in ruleResults)
                {
                    if (result.IsSuccess)
                    {
                        var discountValue = Convert.ToDecimal(result.Rule.SuccessEvent);

                        switch (result.Rule.RuleName)
                        {
                            case "MultiBuySoupBread":
                                ApplyMultiBuySoupBreadDiscount(basket, discountValue);
                                break;

                            case "Apples10PercentDiscount":
                                ApplyApplesDiscount(basket, discountValue);
                                break;
                        }
                    }
                }
            }

            return Result<Basket>.Success(basket);
        }
        catch (Exception ex)
        {
            return Result<Basket>.Failure(ErrorCode.GenericError, ErrorMessages.InvalidRequest);
        }
    }

    private void ApplyMultiBuySoupBreadDiscount(Basket basket, decimal discountValue)
    {
        //var itemLookup = basket.Items.ToDictionary(i => i.ItemName, i => i);

        var soupQuantity = basket.Items
            .Where(i => i.ItemName == "Soup" && i.DiscountAppliedName != "MultiBuySoupBread")
            .Sum(i => i.Quantity);

        var requiredBreads = soupQuantity / 2;
        var breadItemWithDiscount = basket.Items.FirstOrDefault(i => i.ItemName == "Bread" && i.DiscountAppliedName == "MultiBuySoupBread");
        var breadItemWithoutDiscount = basket.Items.FirstOrDefault(i => i.ItemName == "Bread");
        var currentBreads = breadItemWithDiscount?.Quantity ?? 0;
        var breadsToAdd = requiredBreads - currentBreads;

        if (breadsToAdd > 0)
        {
            for (int i = 0; i < breadsToAdd; i++)
            {
                if (breadItemWithDiscount != null)
                {
                    breadItemWithDiscount.Quantity++;
                }
                else
                {
                    if (breadItemWithoutDiscount != null)
                    {
                        breadItemWithoutDiscount.DiscountAppliedName = "MultiBuySoupBread";
                        breadItemWithoutDiscount.DiscountAppliedValue = discountValue;
                    }
                    basket.Items.Add(new BasketItem
                    {
                        ItemName = "Bread",
                        UnitPrice = 0.80m,
                        Quantity = 1,
                        DiscountAppliedValue = discountValue,
                        DiscountAppliedName = "MultiBuySoupBread"
                    });
                }
            }
        }
        else if (breadsToAdd < 0)
        {
            // Remove excess bread items from the basket
            int breadsToRemove = Math.Abs(breadsToAdd);
            if (breadItemWithDiscount != null)
            {
                breadItemWithDiscount.Quantity -= breadsToRemove;
                if (breadItemWithDiscount.Quantity <= 0)
                {
                    basket.Items.Remove(breadItemWithDiscount);
                }
            }
        }
    }

    private void ApplyApplesDiscount(Basket basket, decimal discountValue)
    {
        foreach (var item in basket.Items.Where(i => i.ItemName == "Apples"))
        {
            item.DiscountAppliedValue = discountValue;
            item.DiscountAppliedName = "Apples10PercentDiscount";
        }
    }


    private static decimal GetDiscountFromActionResult(RuleResultTree ruleResult)
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


