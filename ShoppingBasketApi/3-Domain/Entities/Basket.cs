using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; }

    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();

    public decimal Discounts => Items.Sum(item => item.DiscountApplied);

    public decimal Total => Items.Sum(item => item.Price);

    public dynamic[] GetInputs()
    {
        return Items.Select(item => new
        {
            ProductName = item.ItemName,
            Quantity = item.Quantity,
            Price = item.Price
        }).ToArray();
    }

    public BasketDto ToDto()
    {
        return new BasketDto
        {
            UserId = this.UserId,
            Items = this.Items.Select(item => item.ToDto()).ToList()
        };
    }
}
