namespace UnitTests;

using System.Text.Json;
using FluentAssertions;
using Moq;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Entities;
using ShoppingBasketApi.Infrastructure;

public class DiscountRulesEngineTests
{
    private readonly Mock<IRulesFileProvider> mockRulesFileProvider;
    private readonly IRulesEngine rulesEngine;

    private const string TestRulesJsons = @"
    [
        {
        ""WorkflowName"": ""Discounts"",
        ""Rules"": [
            {
            ""RuleName"": ""MultiBuySoupBread"",
            ""SuccessEvent"": ""0.50"",
            ""ErrorMessage"": ""Error applying multi-buy discount"",
            ""Expression"": ""input1.ItemName == \""Soup\"" && input1.Quantity >= 2"",
            ""RuleExpressionType"": 0
            }
        ]
        }
    ]
    ";

    public DiscountRulesEngineTests()
    {
        this.mockRulesFileProvider = new Mock<IRulesFileProvider>();
        this.mockRulesFileProvider.Setup(provider => provider.GetRulesJsonAsync())
            .ReturnsAsync(TestRulesJsons);

        this.rulesEngine = new DiscountRulesEngine(this.mockRulesFileProvider.Object);
    }

    [Fact]
    public async Task ExecuteAllRulesAsync_WhenRulesExist_ReturnsRuleResults()
    {
        // Arrange
        string workflowName = "Discounts";
        BasketInput[] inputs = new BasketInput[]
        {
            new BasketInput{ ItemName = "Soup", Quantity = 2, Price = 50 },
            new BasketInput{ ItemName = "Soup", Quantity = 1, Price = 50 }
        };

        // Act
        var result = await this.rulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().IsSuccess.Should().BeTrue();
        result.First().Rule.RuleName.Should().Be("MultiBuySoupBread");
        result.First().Rule.SuccessEvent.Should().Be("0.50");
    }

    [Fact]
    public void Constructor_WhenRulesJsonIsInvalid_ThrowsJsonException()
    {
        // Arrange
        this.mockRulesFileProvider.Setup(provider => provider.GetRulesJsonAsync())
            .ReturnsAsync("Invalid JSON");

        // Act
        Action act = () => new DiscountRulesEngine(this.mockRulesFileProvider.Object);

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Constructor_WhenRulesFileProviderThrowsFileNotFoundException_ThrowsFileNotFoundException()
    {
        // Arrange
        this.mockRulesFileProvider.Setup(provider => provider.GetRulesJsonAsync())
            .ThrowsAsync(new FileNotFoundException("Rules file not found."));

        // Act
        Action act = () => new DiscountRulesEngine(this.mockRulesFileProvider.Object);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage("Rules file not found.");
    }
}
