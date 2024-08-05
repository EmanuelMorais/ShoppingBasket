namespace ShoppingBasketApi.Application.Dtos;

public record ReceiptDto
{
    public Guid BasketId { get; set; }

    public decimal DiscountsApplied { get; set; }

    public decimal TotalPrice { get; set; }

    public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
}

public record ReceiptItemDto
{
    public string ItemName { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountApplied { get; set; }

    public decimal FinalPrice { get; set; }
}