using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class Basket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; }

    public ICollection<BasketItem> Items { get; set; } = new List<BasketItem>();

    public ICollection<DiscountItem> DiscountItems { get; set; } = new List<DiscountItem>();

    public decimal Discounts => Items.Sum(item => item.FullPrice - item.Price);

    public decimal Total
    {
        get
        {
            decimal itemsTotal = Items.Sum(item => item.Price);
            decimal totalDiscounts = DiscountItems.Sum(item => item.Price);

            return itemsTotal + totalDiscounts;
        }
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
            Items = this.Items.Select(item => item.ToDto()).ToList(),
            DiscountItems = this.DiscountItems.Select(item => item.ToDto()).ToList()
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