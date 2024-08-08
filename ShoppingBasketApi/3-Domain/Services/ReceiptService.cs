using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Domain.Services;

public class ReceiptService : IReceiptService
{
    public Task<Result<Receipt>> GenerateReceipt(Basket basket)
    {
        var receipt = new Receipt
        {
            BasketId = basket.Id,
            TotalPrice = basket.Total,
            DiscountsApplied = basket.Discounts,
            Items = GetItems(basket)
        };
        var totalp = GetItems(basket).Sum(x => x.FinalPrice);
        return Task.FromResult(Result<Receipt>.Success(receipt));
    }

    private static List<ReceiptItem> GetItems(Basket basket)
    {
        var items = basket.Items.Select(item => new ReceiptItem
        {
            ItemName = item.ItemName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            DiscountApplied = item.DiscountAppliedValue,
            FinalPrice = item.Price
        });

        var discountItems = basket.DiscountItems.Select(item => new ReceiptItem
        {
            ItemName = item.ItemName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            DiscountApplied = item.DiscountAppliedValue,
            FinalPrice = item.Price
        });

        return items.Concat(discountItems).ToList();
    }
}

