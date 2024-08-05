using FluentAssertions;
using Moq;
using ShoppingBasketApi.Application;
using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace UnitTests;

public class BasketServiceTests
{
    private readonly Mock<IDiscountService> _mockDiscountService;
    private readonly Mock<IReceiptService> _mockReceiptService;
    private readonly BasketService _basketService;

    public BasketServiceTests()
    {
        _mockDiscountService = new Mock<IDiscountService>();
        _mockReceiptService = new Mock<IReceiptService>();
        _basketService = new BasketService(_mockDiscountService.Object, _mockReceiptService.Object);
    }

    [Fact]
    public async Task CalculateBasketTotalAsync_WhenCalled_ReturnsSuccessWithReceiptDto()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 10 }
            };

        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };
        var receipt = new Receipt
        {
            BasketId = basket.Id,
            DiscountsApplied = 10,
            TotalPrice = 90
        };
        var receiptDto = new ReceiptDto
        {
            BasketId = basket.Id,
            DiscountsApplied = receipt.DiscountsApplied,
            TotalPrice = receipt.TotalPrice,
            Items = new List<ReceiptItemDto>()
        };

        _mockDiscountService
            .Setup(s => s.ApplyBasketDiscountAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Basket>.Success(basket));

        _mockReceiptService
            .Setup(s => s.GenerateReceipt(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Receipt>.Success(receipt));

        // Act
        var result = await _basketService.CalculateBasketTotalAsync(basketItems);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(receiptDto);
    }


    [Fact]
    public async Task CalculateBasketTotalAsync_WhenReceiptServiceFails_ReturnsFailure()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 10 }
            };

        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };
        var receipt = new Receipt
        {
            BasketId = basket.Id,
            DiscountsApplied = 10,
            TotalPrice = 90
        };
        var error = new ApplicationError("RECEIPT_ERROR", "Receipt generation failed");

        _mockDiscountService
            .Setup(s => s.ApplyBasketDiscountAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Basket>.Success(basket));

        _mockReceiptService
            .Setup(s => s.GenerateReceipt(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Receipt>.Failure(error.Code, error.Message));

        // Act
        var result = await _basketService.CalculateBasketTotalAsync(basketItems);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(error);
    }

    [Fact]
    public async Task UpdateBasketWithDiscountsAsync_WhenCalled_ReturnsSuccessWithUpdatedItems()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 10 }
            };

        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

        _mockDiscountService
            .Setup(s => s.ApplyBasketDiscountAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Basket>.Success(basket));

        // Act
        var result = await _basketService.UpdateBasketWithDiscountsAsync(basketItems);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(basketItems);
    }

}