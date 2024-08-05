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
            Items = basket.Items.Select(item => new ReceiptItem
            {
                ItemName = item.ItemName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                DiscountApplied = item.DiscountAppliedValue
            }).ToList()
        };

        return Task.FromResult(Result<Receipt>.Success(receipt));
    }
}

