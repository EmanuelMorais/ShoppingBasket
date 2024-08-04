using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; }

    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();

    public decimal Discounts => Items.Sum(item => (item.UnitPrice * item.Quantity) * item.DiscountAppliedValue);

    public decimal Total => Items.Sum(item => (item.UnitPrice * item.Quantity) * (1 - item.DiscountAppliedValue));

    public BasketInput[] GetInputs()
    {
        return this.Items.Select(item => new BasketInput
        {
            ItemName = item.ItemName,
            Quantity = item.Quantity,
            Price = item.UnitPrice
        }).ToArray();
    }

    public BasketInput[] GetInputs(DateTime currentDate)
    {
        return this.Items.Select(item => new BasketInput
        {
            ItemName = item.ItemName,
            Quantity = item.Quantity,
            Price = item.UnitPrice,
            CurrentDate = currentDate
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
public class BasketInput
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CurrentDate { get; set; }
}