namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using ShoppingBasketApi.Application.Dtos;
    using ShoppingBasketApi.Controllers;
    using ShoppingBasketApi.Domain.Abstractions;
    using ShoppingBasketApi.Domain.Entities;
    using ShoppingBasketApi.Infrastructure.Helpers;
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
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 10 }
            };

            var receipt = new ReceiptDto { BasketId = Guid.NewGuid(), DiscountsApplied = 10, TotalPrice = 90 };
            _mockBasketService
                .Setup(service => service.CalculateBasketTotalAsync(basketItems))
                .ReturnsAsync(Result<ReceiptDto>.Success(receipt));

            // Act
            var result = await _controller.CalculateTotal(basketItems);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(new { Receipt = receipt });
        }

        [Fact]
        public async Task CalculateTotal_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            // Arrange
            var basketItems = new List<BasketItemDto>();

            // Configure the mock to return a failure Result
            _mockBasketService
                .Setup(service => service.CalculateBasketTotalAsync(basketItems))
                .ReturnsAsync(Result<ReceiptDto>.Failure("InvalidData", "Invalid data"));

            // Act
            var result = await _controller.CalculateTotal(basketItems);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().BeEquivalentTo(new { Code = "InvalidData", Message = "Invalid data" });
        }

        [Fact]
        public async Task UpdateBasket_WhenCalled_ReturnsOkResultWithUpdatedItems()
        {
            // Arrange
            var basketItems = new List<BasketItemDto>
            {
                new BasketItemDto { ItemName = "Item1", Quantity = 1, UnitPrice = 100, DiscountAppliedValue = 10 }
            };

            var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

            _mockBasketService
                .Setup(service => service.UpdateBasketWithDiscountsAsync(basketItems, false))
                .ReturnsAsync(Result<BasketDto>.Success(basket.ToDto()));

            // Act
            var result = await _controller.UpdateBasket(basketItems);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(basketItems);
        }

        [Fact]
        public async Task UpdateBasket_WhenServiceReturnsFailure_ReturnsBadRequest()
        {
            // Arrange
            var basketItems = new List<BasketItemDto>();

            var basket = new Basket { Items = basketItems.Select(item => item.ToDomain()).ToList() };

            _mockBasketService
                .Setup(service => service.UpdateBasketWithDiscountsAsync(basketItems, false))
                .ReturnsAsync(Result<BasketDto>.Success(basket.ToDto()));

            // Act
            var result = await _controller.UpdateBasket(basketItems);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().BeEquivalentTo(new { Code = "InvalidData", Message = "Invalid data" });
        }
    }
}
