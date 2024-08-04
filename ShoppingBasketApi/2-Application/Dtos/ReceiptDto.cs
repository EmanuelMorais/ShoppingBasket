namespace ShoppingBasketApi.Application.Dtos;

public record ReceiptDto
{
    public Guid BasketId { get; set; }

    public decimal DiscountsApplied { get; set; }

    public decimal TotalPrice { get; set; }
}