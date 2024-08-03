using FluentAssertions;
using ShoppingBasketApi.Infrastructure;

public class DiscountRulesEngineTests
{
    private const string TestRulesJson = @"
        [
            {
                ""Name"": ""Discounts"",
                ""Rules"": [
                    {
                        ""RuleName"": ""TestRule"",
                        ""SuccessEvent"": ""0.1"",
                        ""ErrorMessage"": """",
                        ""Expression"": ""true""
                    }
                ]
            }
        ]";

    private DiscountRulesEngine _discountRulesEngine;

    public DiscountRulesEngineTests()
    {
        // Set up the test environment to simulate reading rules from a JSON string
        File.WriteAllText(GetRulesFilePath(), TestRulesJson);
        _discountRulesEngine = new DiscountRulesEngine();
    }

    [Fact]
    public async Task ExecuteAllRulesAsync_WhenRulesExist_ReturnsRuleResults()
    {
        // Arrange
        string workflowName = "Discounts";
        dynamic[] inputs = new[]
        {
                new { ProductName = "Item1", Quantity = 2, UnitPrice = 50 },
                new { ProductName = "Item2", Quantity = 1, UnitPrice = 100 }
            };

        // Act
        var result = await _discountRulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().IsSuccess.Should().BeTrue();
        result.First().Rule.RuleName.Should().Be("TestRule");
        result.First().Rule.SuccessEvent.Should().Be("0.1");
    }

    [Fact]
    public async Task ExecuteAllRulesAsync_WhenNoWorkflowFound_ThrowsArgumentException()
    {
        // Arrange
        string workflowName = "NonExistentWorkflow";
        dynamic[] inputs = new[]
        {
                new { ProductName = "Item1", Quantity = 2, UnitPrice = 50 },
                new { ProductName = "Item2", Quantity = 1, UnitPrice = 100 }
            };

        // Act
        Func<Task> act = async () => await _discountRulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
           .WithMessage($"No configuration found for workflow: {workflowName}");
    }

    [Fact]
    public async Task ExecuteAllRulesAsync_WhenRulesFileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        string workflowName = "Discounts";
        dynamic[] inputs = new[]
        {
                new { ProductName = "Item1", Quantity = 2, UnitPrice = 50 },
                new { ProductName = "Item2", Quantity = 1, UnitPrice = 100 }
            };

        // Act
        // Temporarily rename the file to simulate file not found.
        File.Move(GetRulesFilePath(), GetRulesFilePath() + ".bak");

        Func<Task> act = async () => await _discountRulesEngine.ExecuteAllRulesAsync(workflowName, inputs);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("Rules file not found");

        // Clean up by renaming the file back.
        File.Move(GetRulesFilePath() + ".bak", GetRulesFilePath());
    }

    private string GetRulesFilePath()
    {
        // Adjust this path according to your test setup
        return Path.Combine(AppContext.BaseDirectory, "rules.json");
    }
}
