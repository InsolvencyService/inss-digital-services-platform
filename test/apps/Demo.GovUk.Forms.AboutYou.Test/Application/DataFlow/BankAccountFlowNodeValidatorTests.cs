using System.ComponentModel.DataAnnotations;
using Demo.GovUk.Forms.AboutYou.Application.DataFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.DataFlow;

public class BankAccountFlowNodeValidatorTests
{
    [Fact]
    public async Task InvalidBankAccountDetails_ValidateAsync_ReturnsErrorDetails()
    {
        BankAccountFlowNodeValidator validator = new();
        BankAccountModel bankAccount = new() { AccountName = "H J Simpson", AccountNumber = "12345678", SortCode = "11-22-33" };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankAccount.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankAccount };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "The bank account details are invalid", "AccountNumber");
        AssertError(validationResults[0], "The bank account details are invalid", "SortCode");
    }
    
    [Fact]
    public async Task InvalidBuildingSocietyRollNumber_ValidateAsync_ReturnsErrorDetails()
    {
        BankAccountFlowNodeValidator validator = new();
        BankAccountModel bankAccount = new()
        {
            AccountName = "H J Simpson",
            AccountNumber = "11223344", 
            SortCode = "11-22-33", 
            BuildingSocietyRollNumber = "ABC-123$"
        };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankAccount.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankAccount };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "Building society roll number must only include letters a to z, numbers, " +
                                          "hyphens, spaces, forward slashes and full stops", "BuildingSocietyRollNumber");
    }
    
    [Fact]
    public async Task ValidBankAccountDetails_ValidateAsync_ReturnsNoErrorDetails()
    {
        BankAccountFlowNodeValidator validator = new();
        BankAccountModel bankAccount = new() { AccountName = "H J Simpson", AccountNumber = "11223344", SortCode = "11-22-33" };
        FlowNode node = new() { Id = "NodeId1", PagePath = bankAccount.Path };
        FlowNodeContext context = new() { Nodes = [node], CurrentNode = node, CurrentPage = bankAccount };
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Empty(validationResults);
    }
    
    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.NotNull(result.MemberNames.FirstOrDefault(p => p == property));
    }
}