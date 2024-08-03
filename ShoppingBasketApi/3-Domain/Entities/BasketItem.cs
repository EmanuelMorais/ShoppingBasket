using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class BasketItem
{
    public string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price => UnitPrice * Quantity;

    public decimal DiscountApplied { get; set; }

    public BasketItemDto ToDto()
    {
        return new BasketItemDto
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            DiscountApplied = this.DiscountApplied
        };
    }
}


