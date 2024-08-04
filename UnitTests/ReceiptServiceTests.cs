using FluentAssertions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Domain.Services;

namespace UnitTests;

public class ReceiptServiceTests
{
    private readonly ReceiptService _receiptService;

    public ReceiptServiceTests()
    {
        _receiptService = new ReceiptService();
    }

    [Fact]
    public async Task GenerateReceipt_WhenCalled_ReturnsReceiptWithBasketDetails()
    {
        // Arrange
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Item1", Quantity = 2, UnitPrice = 50, DiscountAppliedValue = 5 },
                    new BasketItem { ItemName = "Item2", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 0 }
                }
        };

        // Act
        var result = await _receiptService.GenerateReceipt(basket);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BasketId.Should().Be(basket.Id);
        result.Value.TotalPrice.Should().Be(basket.Total);
        result.Value.DiscountsApplied.Should().Be(basket.Discounts);
    }
}

