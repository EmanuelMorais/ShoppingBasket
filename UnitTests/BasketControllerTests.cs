namespace UnitTests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingBasketApi.Application.Dtos;
using ShoppingBasketApi.Controllers;
using ShoppingBasketApi.Domain.Abstractions;
using Xunit;

public class BasketControllerTests
{
    private readonly Mock<IBasketService> _mockBasketService;
    private readonly BasketController _controller;

    public BasketControllerTests()
    {
        _mockBasketService = new Mock<IBasketService>();
        _controller = new BasketController(_mockBasketService.Object);
    }

    [Fact]
    public async Task CalculateTotal_WhenCalled_ReturnsOkResultWithReceipt()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
            };

        var receipt = new ReceiptDto { BasketId = Guid.NewGuid(), DiscountsApplied = 10, TotalPrice = 90 };
        _mockBasketService
            .Setup(service => service.CalculateBasketTotalAsync(basketItems))
            .ReturnsAsync(receipt);

        // Act
        var result = await _controller.CalculateTotal(basketItems);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { Receipt = receipt });
    }

    [Fact]
    public async Task CalculateTotal_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>();

        _mockBasketService
            .Setup(service => service.CalculateBasketTotalAsync(basketItems))
            .ThrowsAsync(new Exception("Invalid data"));

        // Act
        var result = await _controller.CalculateTotal(basketItems);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().BeEquivalentTo(new { Message = "Invalid data" });
    }

    [Fact]
    public async Task UpdateBasket_WhenCalled_ReturnsOkResultWithUpdatedItems()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountApplied = 10 }
            };

        _mockBasketService
            .Setup(service => service.UpdateBasketWithDiscountsAsync(basketItems))
            .ReturnsAsync(basketItems);

        // Act
        var result = await _controller.UpdateBasket(basketItems);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(basketItems);
    }

    [Fact]
    public async Task UpdateBasket_WhenServiceThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var basketItems = new List<BasketItemDto>();

        _mockBasketService
            .Setup(service => service.UpdateBasketWithDiscountsAsync(basketItems))
            .ThrowsAsync(new Exception("Invalid data"));

        // Act
        var result = await _controller.UpdateBasket(basketItems);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().BeEquivalentTo(new { Message = "Invalid data" });
    }
}

