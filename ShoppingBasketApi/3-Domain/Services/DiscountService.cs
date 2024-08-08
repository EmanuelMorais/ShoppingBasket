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
    private const string BreadOffer = "Bread(offer)";

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
            var groupedItems = basketItems
                .GroupBy(x => x.ItemName)
                .Select(g => new BasketItem
                {
                    ItemName = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    UnitPrice = g.First().UnitPrice,
                    DiscountAppliedValue = 0
                })
                .ToList();

            foreach (var basketItem in groupedItems)
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
        var soups = basket.Items
            .Where(i => i.ItemName == Soup);

        var soupsWithDiscount = soups
            .Where(i => i.DiscountAppliedName == MultiBuySoupBread)
            .Sum(i => i.Quantity);

        var soupQuantity = soups
            .Where(i => i.DiscountAppliedName != MultiBuySoupBread)
            .Sum(i => i.Quantity);

        if (soupQuantity == soupsWithDiscount)
        {
            return;
        }

        var requiredBreads = soupQuantity / 2;

        var breadItemWithDiscount = basket
            .DiscountItems
            .FirstOrDefault(i =>
                i.ItemName == BreadOffer &&
                i.DiscountAppliedName == MultiBuySoupBread &&
                i.Quantity > 0 &&
                !i.ForceRemove);

        var currentBreads = breadItemWithDiscount?.Quantity ?? 0;
        var breadsToAdd = GetBreadsToAdd(requiredBreads, currentBreads, basket.DiscountItems);

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

                    basket.DiscountItems.Add(new DiscountItem
                    {
                        ItemName = BreadOffer,
                        UnitPrice = 0.80m,
                        Quantity = 1,
                        DiscountAppliedValue = discountValue,
                        DiscountAppliedName = MultiBuySoupBread
                    });
                }
            }

            var freshSoups = soups.Where(i => i.DiscountAppliedName != MultiBuySoupBread);

            foreach (var soup in freshSoups)
            {
                soup.DiscountAppliedName = MultiBuySoupBread;
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
                    basket.DiscountItems.Remove(breadItemWithDiscount);
                }
            }
        }
    }

    private static int GetBreadsToAdd(int requiredBreads, int currentBreads, IEnumerable<DiscountItem> items)
    {
        var breadsToAdd = requiredBreads - currentBreads;
        var breadsToRemove = items
            .Where(i =>
                i.ItemName == BreadOffer &&
                i.DiscountAppliedName == MultiBuySoupBread &&
                i.Quantity == 0 &&
                i.ForceRemove)
            .Count();

        return breadsToAdd - breadsToRemove;
    }

    private static void ApplyApplesDiscount(Basket basket, decimal discountValue)
    {
        var apples = basket.Items.Where(i => i.ItemName == Apples);

        foreach (var item in apples)
        {
            item.DiscountAppliedValue = discountValue;
            item.DiscountAppliedName = Apples10PercentDiscount;
        }
    }
}


