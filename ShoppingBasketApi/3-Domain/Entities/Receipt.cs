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
            BasketId = this.BasketId,
            DiscountsApplied = this.DiscountsApplied,
            TotalPrice = this.TotalPrice,
            Items = this.Items.Select(i => i.ToDto()).ToList()
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
            ItemName = this.ItemName,
            UnitPrice = this.UnitPrice,
            Quantity = this.Quantity,
            DiscountApplied = this.DiscountApplied,
            FinalPrice = this.FinalPrice
        };
    }
}

