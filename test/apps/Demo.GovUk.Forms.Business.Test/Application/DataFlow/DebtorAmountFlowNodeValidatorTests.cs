using System.ComponentModel.DataAnnotations;
using Demo.GovUk.Forms.Business.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.Business.Test.Application.DataFlow;

public class DebtorAmountFlowNodeValidatorTests
{
    [Theory]
    [InlineData(999)]
    [InlineData(100_001)]
    public async Task InvalidDebtorAmount_ValidateAsync_ReturnsErrorDetails(int amount)
    {
        DebtorAmountFlowNodeValidator validator = new();
        MoneyModel money = new() { Amount = amount };
        FlowNode node = new() { Id = "NodeId1", PagePath = money.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = money };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "Only add debtors who owe between £1,000 and £100,000 inclusive", "Amount");
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(10_000)]
    [InlineData(100_000)]
    public async Task ValidDebtorAmount_ValidateAsync_ReturnsNoErrorDetails(int amount)
    {
        DebtorAmountFlowNodeValidator validator = new();
        MoneyModel money = new() { Amount = amount };
        FlowNode node = new() { Id = "NodeId1", PagePath = money.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = money };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Empty(validationResults);
    }
    
    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.NotNull(result.MemberNames.FirstOrDefault(p => p == property));
    }
}