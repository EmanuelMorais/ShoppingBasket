using FluentAssertions;
using Moq;
using ShoppingBasketApi.Application;
using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure.Entities;
using ShoppingBasketApi.Infrastructure.Helpers;

namespace ShoppingBasketApi.Tests.Services;
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
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
            };

        var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };
        var receipt = new Receipt
        {

            DiscountsApplied = 10,
            TotalPrice = 90
        };
        var receiptDto = new ReceiptDto
        {
            BasketId = basket.Id,
            DiscountsApplied = receipt.DiscountsApplied,
            TotalPrice = receipt.TotalPrice
        };

        _mockDiscountService
            .Setup(s => s.ApplyDiscountsAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Receipt>.Success(receipt));

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
    public async Task CalculateBasketTotalAsync_WhenDiscountServiceFails_ReturnsFailure()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
            };

        var error = new ApplicationError("DISCOUNT_ERROR", "Discount calculation failed");

        _mockDiscountService
            .Setup(s => s.ApplyDiscountsAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Receipt>.Failure(error.Code, error.Message));

        // Act
        var result = await _basketService.CalculateBasketTotalAsync(basketItems);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(error);
    }

    [Fact]
    public async Task CalculateBasketTotalAsync_WhenReceiptServiceFails_ReturnsFailure()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
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
            .Setup(s => s.ApplyDiscountsAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Receipt>.Success(receipt));

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
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
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

    [Fact]
    public async Task UpdateBasketWithDiscountsAsync_WhenDiscountServiceFails_ReturnsFailure()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
            };

        var error = new ApplicationError("DISCOUNT_ERROR", "Discount application failed");

        _mockDiscountService
            .Setup(s => s.ApplyBasketDiscountAsync(It.IsAny<Basket>()))
            .ReturnsAsync(Result<Basket>.Failure(error.Code, error.Message));

        // Act
        var result = await _basketService.UpdateBasketWithDiscountsAsync(basketItems);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(error);
    }
}