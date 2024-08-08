using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Application.Dtos;

public record BasketItemDto : ItemDto
{
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price { get; set; }

    public decimal FullPrice { get; set; }

    public decimal DiscountAppliedValue { get; set; }

    public string DiscountAppliedName { get; set; }

    public bool ForceRemove { get; set; }

    public BasketItem ToDomain()
    {
        return new BasketItem
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            DiscountAppliedValue = this.DiscountAppliedValue,
            DiscountAppliedName = this.DiscountAppliedName,
            ForceRemove = this.ForceRemove
        };
    }
}


