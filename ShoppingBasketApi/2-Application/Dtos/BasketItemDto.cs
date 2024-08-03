using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Application.Dtos;

public record BasketItemDto
{
    public required string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price => UnitPrice * Quantity;

    public decimal DiscountApplied { get; set; }

    public BasketItem ToDomain()
    {
        return new BasketItem
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            DiscountApplied = this.DiscountApplied
        };
    }
}


