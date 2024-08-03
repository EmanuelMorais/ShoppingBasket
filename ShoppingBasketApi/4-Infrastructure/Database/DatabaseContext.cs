using System;
using Microsoft.EntityFrameworkCore;
using ShoppingBasketApi.Domain.Entities;

namespace ShoppingBasketApi.Data.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Rule> Rules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Workflows
            modelBuilder.Entity<Workflow>().HasData(
                new Workflow { Id = 1, Name = "Discounts" }
            );

            // Seed Rules with the corresponding WorkflowId
            modelBuilder.Entity<Rule>().HasData(
                new Rule
                {
                    Id = 1,
                    RuleName = "Apples10PercentDiscount",
                    SuccessEvent = "Apply 10% discount to apples",
                    Expression = "ProductName == \"Apples\"",
                    DiscountAmount = 0.10m,
                    WorkflowId = 1,
                    ErrorMessage = "Error applying discount to apples",
                    RuleExpressionType = "LambdaExpression"
                },
                new Rule
                {
                    Id = 2,
                    RuleName = "MultiBuySoupBread",
                    SuccessEvent = "Apply multi-buy discount",
                    Expression = "ProductName == \"Soup\" && Quantity >= 2",
                    DiscountAmount = 0.40m,
                    WorkflowId = 1,
                    ErrorMessage = "Error applying multi-buy discount",
                    RuleExpressionType = "LambdaExpression"
                }
            );
        }

    }
}

