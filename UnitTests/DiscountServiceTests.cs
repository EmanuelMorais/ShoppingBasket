using FluentAssertions;
using Moq;
using RulesEngine.Models;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Domain.Services;
using ShoppingBasketApi.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class DiscountServiceTests
    {
        private readonly Mock<IRulesEngine> _mockRulesEngine;
        private readonly DiscountService _discountService;

        public DiscountServiceTests()
        {
            _mockRulesEngine = new Mock<IRulesEngine>();
            _discountService = new DiscountService(_mockRulesEngine.Object);
        }

        [Fact]
        public async Task ApplyDiscountsAsync_ValidBasket_ReturnsSuccessWithCorrectReceipt()
        {
            // Arrange
            var basket = new Basket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Item1", Quantity = 1, UnitPrice = 100 }
                }
            };

            var ruleResults = new List<RuleResultTree>
            {
                new RuleResultTree
                {
                    IsSuccess = true,
                    Rule = new RulesEngine.Models.Rule
                    {
                        RuleName = "DiscountRule1",
                        SuccessEvent = "0.10" // 10% discount
                    }
                }
            };

            _mockRulesEngine
                .Setup(r => r.ExecuteAllRulesAsync("Discounts", It.IsAny<BasketInput[]>()))
                .ReturnsAsync(ruleResults);

            var expectedReceipt = new Receipt
            {
                BasketId = basket.Id,
                DiscountsApplied = 10m, // 10% of 100
                TotalPrice = 90m // 100 - 10
            };

            // Act
            var result = await _discountService.ApplyDiscountsAsync(basket);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedReceipt);
        }

        [Fact]
        public async Task ApplyDiscountsAsync_NoApplicableRules_ReturnsSuccessWithNoDiscounts()
        {
            // Arrange
            var basket = new Basket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Item2", Quantity = 2, UnitPrice = 200 }
                }
            };

            var ruleResults = new List<RuleResultTree>(); // No rules applied

            _mockRulesEngine
                .Setup(r => r.ExecuteAllRulesAsync("Discounts", It.IsAny<BasketInput[]>()))
                .ReturnsAsync(ruleResults);

            var expectedReceipt = new Receipt
            {
                BasketId = basket.Id,
                DiscountsApplied = 0m,
                TotalPrice = 400m // 200 * 2
            };

            // Act
            var result = await _discountService.ApplyDiscountsAsync(basket);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedReceipt);
        }

        [Fact]
        public async Task ApplyDiscountsAsync_RulesEngineFailure_ReturnsFailure()
        {
            // Arrange
            var basket = new Basket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Item3", Quantity = 1, UnitPrice = 150 }
                }
            };

            _mockRulesEngine
                .Setup(r => r.ExecuteAllRulesAsync("Discounts", It.IsAny<BasketInput[]>()))
                .ThrowsAsync(new Exception("Rules engine failure"));

            // Act
            var result = await _discountService.ApplyDiscountsAsync(basket);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().BeEquivalentTo(ErrorCode.GenericError);
            result.Error.Message.Should().BeEquivalentTo(ErrorMessages.InvalidRequest);
        }

        [Fact]
        public async Task ApplyBasketDiscountAsync_ValidBasket_ReturnsSuccessWithCorrectBasket()
        {
            // Arrange
            var basket = new Basket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Soup", Quantity = 3, UnitPrice = 1.0m },
                    new BasketItem { ItemName = "Bread", Quantity = 1, UnitPrice = 0.8m }
                }
            };

            var ruleResults = new List<RuleResultTree>
            {
                new RuleResultTree
                {
                    IsSuccess = true,
                    Rule = new RulesEngine.Models.Rule
                    {
                        RuleName = "MultiBuySoupBread",
                        SuccessEvent = "0.80"
                    }
                }
            };

            _mockRulesEngine
                .Setup(r => r.ExecuteAllRulesAsync("Discounts", It.IsAny<BasketItemInput[]>()))
                .ReturnsAsync(ruleResults);

            // Act
            var result = await _discountService.ApplyBasketDiscountAsync(basket);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var breadItems = basket.Items.Where(i => i.ItemName == "Bread").ToList();
            breadItems.Count.Should().Be(2);
            breadItems.First().DiscountAppliedName.Should().Be("MultiBuySoupBread");
            breadItems.First().DiscountAppliedValue.Should().Be(0.80m);
        }

        [Fact]
        public async Task ApplyBasketDiscountAsync_RulesEngineFailure_ReturnsFailure()
        {
            // Arrange
            var basket = new Basket
            {
                Items = new List<BasketItem>
                {
                    new BasketItem { ItemName = "Apples", Quantity = 1, UnitPrice = 1.0m }
                }
            };

            _mockRulesEngine
                .Setup(r => r.ExecuteAllRulesAsync("Discounts", It.IsAny<BasketInput[]>()))
                .ThrowsAsync(new Exception("Rules engine failure"));

            // Act
            var result = await _discountService.ApplyBasketDiscountAsync(basket);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().BeEquivalentTo(ErrorCode.GenericError);
            result.Error.Message.Should().BeEquivalentTo(ErrorMessages.InvalidRequest);
        }
    }
}
