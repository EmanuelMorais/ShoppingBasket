using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class DiscountItem : Item
{
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal FullPrice => UnitPrice * Quantity;

    public decimal Price
    {
        get
        {
            decimal discountMultiplier = 1 - (DiscountAppliedValue / 100);

            return Math.Round(FullPrice * discountMultiplier, 2, MidpointRounding.AwayFromZero);
        }
    }

    public decimal DiscountAppliedValue { get; set; }

    public string DiscountAppliedName { get; set; }

    public bool ForceRemove { get; set; }

    public DiscountItemDto ToDto()
    {
        return new DiscountItemDto
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            Price = this.Price,
            FullPrice = this.FullPrice,
            DiscountAppliedValue = this.DiscountAppliedValue,
            DiscountAppliedName = this.DiscountAppliedName,
            ForceRemove = this.ForceRemove
        };
    }
}

