using ShoppingBasketApi.Application.Dtos;

namespace ShoppingBasketApi.Domain.Entities;

public class Receipt
{
    public Guid BasketId { get; set; }

    public decimal DiscountsApplied { get; set; }

    public decimal TotalPrice { get; set; }

    public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();

    public ReceiptDto ToDto()
    {
        return new ReceiptDto
        {
            BasketId = BasketId,
            DiscountsApplied = DiscountsApplied,
            TotalPrice = TotalPrice,
            Items = Items.Select(i => i.ToDto()).ToList()
        };
    }
}


public class ReceiptItem
{
    public string ItemName { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountApplied { get; set; }

    public decimal FinalPrice { get; set; }

    public ReceiptItemDto ToDto()
    {
        return new ReceiptItemDto
        {
            ItemName = ItemName,
            UnitPrice = UnitPrice,
            Quantity = Quantity,
            DiscountApplied = DiscountApplied,
            FinalPrice = FinalPrice
        };
    }
}

