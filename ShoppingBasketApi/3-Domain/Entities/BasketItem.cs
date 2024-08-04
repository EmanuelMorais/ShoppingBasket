using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class BasketItem
{
    public string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price => DiscountAppliedValue > 0m ? (this.UnitPrice * this.Quantity) * DiscountAppliedValue : this.UnitPrice * this.Quantity;

    public decimal FullPrice => this.UnitPrice * this.Quantity;

    public decimal DiscountAppliedValue { get; set; }

    public string DiscountAppliedName { get; set; }


    public BasketItemDto ToDto()
    {
        return new BasketItemDto
        {
            ItemName = this.ItemName,
            Quantity = this.Quantity,
            UnitPrice = this.UnitPrice,
            DiscountAppliedValue = this.DiscountAppliedValue,
            DiscountAppliedName = this.DiscountAppliedName
        };
    }

    public BasketItemInput[] GetInputs()
    {
        return new BasketItemInput[]
        {
            new BasketItemInput
            {
                ItemName = this.ItemName,
                Quantity = this.Quantity,
                Price = this.UnitPrice
            }
        };
    }

    public BasketItemInput[] GetInputs(DateTime currentDate)
    {
        return new BasketItemInput[]
        {
            new BasketItemInput
            {
                ItemName = this.ItemName,
                Quantity = this.Quantity,
                Price = this.UnitPrice,
                CurrentDate = currentDate
            }
        };
    }
}

public class BasketItemInput
{
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime CurrentDate { get; set; }
}