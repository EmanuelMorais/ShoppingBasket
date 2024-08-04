using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Application.Dtos;

public record BasketItemDto
{
    public required string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price => DiscountAppliedValue > 0m ? (this.UnitPrice * this.Quantity) * DiscountAppliedValue : this.UnitPrice * this.Quantity;

    public decimal FullPrice => this.UnitPrice * this.Quantity;

    public decimal DiscountAppliedValue { get; set; }

    public string DiscountAppliedName { get; set; }

    public BasketItem ToDomain()
    {
        return new BasketItem
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            DiscountAppliedValue = this.DiscountAppliedValue,
            DiscountAppliedName = this.DiscountAppliedName
        };
    }
}


