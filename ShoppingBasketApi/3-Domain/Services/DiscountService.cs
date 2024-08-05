using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Services;

public class DiscountService : IDiscountService
{
    private readonly IRulesEngine rulesEngine;
    private const string Discounts = "Discounts";
    private const string MultiBuySoupBread = "MultiBuySoupBread";
    private const string Apples10PercentDiscount = "Apples10PercentDiscount";
    private const string Soup = "Soup";
    private const string Bread = "Bread";
    private const string Apples = "Apples";

    public DiscountService(IRulesEngine rulesEngine)
    {
        this.rulesEngine = rulesEngine;
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
                var ruleResults = await this.rulesEngine.ExecuteAllRulesAsync(Discounts, inputs);

                if (ruleResults is null)
                {
                    return Result<Basket>.Success(basket);
                }

                foreach (var result in ruleResults)
                {
                    if (result.IsSuccess)
                    {
                        var discountValue = Convert.ToDecimal(result.Rule.SuccessEvent);

                        switch (result.Rule.RuleName)
                        {
                            case MultiBuySoupBread:
                                ApplyMultiBuySoupBreadDiscount(basket, discountValue);
                                break;

                            case Apples10PercentDiscount:
                                ApplyApplesDiscount(basket, discountValue);
                                break;
                        }
                    }
                }
            }

            return Result<Basket>.Success(basket);
        }
        catch (Exception)
        {
            return Result<Basket>.Failure(ErrorCode.GenericError, ErrorMessages.InvalidRequest);
        }
    }

    private static void ApplyMultiBuySoupBreadDiscount(Basket basket, decimal discountValue)
    {
        var soupQuantity = basket.Items
            .Where(i => i.ItemName == Soup && i.DiscountAppliedName != MultiBuySoupBread)
            .Sum(i => i.Quantity);

        var requiredBreads = soupQuantity / 2;
        var breadItemWithDiscount = basket.Items.FirstOrDefault(i => i.ItemName == Bread && i.DiscountAppliedName == MultiBuySoupBread);
        var breadItemWithoutDiscount = basket.Items.FirstOrDefault(i => i.ItemName == Bread && string.IsNullOrEmpty(i.DiscountAppliedName));
        var currentBreads = breadItemWithDiscount?.Quantity ?? 0;
        var breadsToAdd = requiredBreads - currentBreads;

        if (breadsToAdd > 0)
        {
            for (int i = 0; i < breadsToAdd; i++)
            {
                if (breadItemWithDiscount != null)
                {
                    breadItemWithDiscount.Quantity += breadsToAdd;
                }
                else
                {
                    if (breadItemWithoutDiscount != null)
                    {
                        breadItemWithoutDiscount.DiscountAppliedName = MultiBuySoupBread;
                        breadItemWithoutDiscount.DiscountAppliedValue = discountValue;
                    }
                    else
                    {
                        basket.Items.Add(new BasketItem
                        {
                            ItemName = Bread,
                            UnitPrice = 0.80m,
                            Quantity = 1,
                            DiscountAppliedValue = discountValue,
                            DiscountAppliedName = MultiBuySoupBread
                        });
                    }
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

    private static void ApplyApplesDiscount(Basket basket, decimal discountValue)
    {
        foreach (var item in basket.Items.Where(i => i.ItemName == Apples))
        {
            item.DiscountAppliedValue = discountValue;
            item.DiscountAppliedName = Apples10PercentDiscount;
        }
    }
}


