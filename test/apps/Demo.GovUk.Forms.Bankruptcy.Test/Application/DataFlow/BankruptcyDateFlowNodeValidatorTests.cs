using System.ComponentModel.DataAnnotations;
using Demo.GovUk.Forms.Bankruptcy.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.Bankruptcy.Test.Application.DataFlow;

public class BankruptcyDateFlowNodeValidatorTests
{
    [Fact]
    public async Task InvalidDate_ValidateAsync_ReturnsErrorDetails()
    {
        BankruptcyDateFlowNodeValidator validator = new();
        DateModel bankruptcyDate = new() { Day = 29, Month = 2, Year = 2026 };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankruptcyDate.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankruptcyDate };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "Enter a valid date", "Value");
    }
    
    [Fact]
    public async Task InvalidAfterToday_ValidateAsync_ReturnsErrorDetails()
    {
        BankruptcyDateFlowNodeValidator validator = new();
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateModel bankruptcyDate = new() { Day = today.Day, Month = today.Month, Year = today.Year };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankruptcyDate.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankruptcyDate };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "The bankruptcy date must be in the past 12 months", "Value");
    }
    
    [Fact]
    public async Task InvalidOver12MonthsAgo_ValidateAsync_ReturnsErrorDetails()
    {
        BankruptcyDateFlowNodeValidator validator = new();
        DateOnly over12MonthsAgo = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1).AddDays(-1);
        DateModel bankruptcyDate = new() { Day = over12MonthsAgo.Day, Month = over12MonthsAgo.Month, Year = over12MonthsAgo.Year };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankruptcyDate.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankruptcyDate };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "The bankruptcy date must be in the past 12 months", "Value");
    }
    
    [Fact]
    public async Task DateBeforeAfterToday_ValidateAsync_ReturnsNoErrorDetails()
    {
        BankruptcyDateFlowNodeValidator validator = new();
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        DateModel bankruptcyDate = new() { Day = today.Day, Month = today.Month, Year = today.Year };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankruptcyDate.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankruptcyDate };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Empty(validationResults);
    }
    
    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.NotNull(result.MemberNames.FirstOrDefault(p => p == property));
    }
}