﻿using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class BasketItem : Item
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

    public BasketItemDto ToDto()
    {
        return new BasketItemDto
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