using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Application.Dtos;

public record BasketDto
{
    public string UserId { get; set; }

    public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();

    public List<DiscountItemDto> DiscountItems { get; set; } = new List<DiscountItemDto>();

    public decimal Discounts => Items.Sum(item => (item.UnitPrice * item.Quantity) * item.DiscountAppliedValue);

    public decimal Total
    {
        get
        {
            var itemsTotal = Items.Sum(item => item.Price);
            var totalDiscounts = DiscountItems.Sum(item => item.Price);

            return itemsTotal + totalDiscounts;
        }
    }

    public Basket ToDomain()
    {
        return new Basket
        {
            UserId = this.UserId,
            Items = this.Items.Select(item => item.ToDomain()).ToList(),
            DiscountItems = this.DiscountItems.Select(item => item.ToDomain()).ToList()
        };
    }
}
