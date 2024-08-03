using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Application.Dtos;

public record BasketDto
{
    public string UserId { get; set; }

    public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();

    public decimal Discounts => Items.Sum(item => item.DiscountApplied);

    public decimal Total => Items.Sum(item => item.Price);

    public Basket ToDomain()
    {
        return new Basket
        {
            UserId = this.UserId,
            Items = this.Items.Select(item => item.ToDomain()).ToList()
        };
    }
}
