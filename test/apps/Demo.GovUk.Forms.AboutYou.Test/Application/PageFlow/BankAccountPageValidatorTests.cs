using System.ComponentModel.DataAnnotations;
using Demo.GovUk.Forms.AboutYou.Application.PageFlow;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.AboutYou.Test.Application.PageFlow;

public class BankAccountPageValidatorTests
{
    [Fact]
    public async Task InvalidBankAccountDetails_ValidateAsync_ReturnsErrorDetails()
    {
        BankAccountPageValidator validator = new();
        BankAccountModel bankAccount = new() { AccountName = "H J Simpson", AccountNumber = "12345678", SortCode = "11-22-33" };
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = bankAccount.Path });
        ValidatePageContext context = new() { CurrentNode = node, CurrentPage = bankAccount };
        
        await validator.ValidateAsync(context);

        Assert.Single(context.ValidationResults);
        AssertError(context.ValidationResults[0], "The bank account details are invalid", "AccountNumber");
        AssertError(context.ValidationResults[0], "The bank account details are invalid", "SortCode");
    }
    
    [Fact]
    public async Task InvalidBuildingSocietyRollNumber_ValidateAsync_ReturnsErrorDetails()
    {
        BankAccountPageValidator validator = new();
        BankAccountModel bankAccount = new()
        {
            AccountName = "H J Simpson",
            AccountNumber = "11223344", 
            SortCode = "11-22-33", 
            BuildingSocietyRollNumber = "ABC-123$"
        };
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = bankAccount.Path });
        ValidatePageContext context = new() { CurrentNode = node, CurrentPage = bankAccount };
        
        await validator.ValidateAsync(context);

        Assert.Single(context.ValidationResults);
        AssertError(context.ValidationResults[0], "Building society roll number must only include letters a to z, numbers, " +
                                                  "hyphens, spaces, forward slashes and full stops", "BuildingSocietyRollNumber");
    }
    
    [Fact]
    public async Task ValidBankAccountDetails_ValidateAsync_ReturnsNoErrorDetails()
    {
        BankAccountPageValidator validator = new();
        BankAccountModel bankAccount = new() { AccountName = "H J Simpson", AccountNumber = "11223344", SortCode = "11-22-33" };
        TreeNode node = new(new FlowNode { Id = "NodeId1", PagePath = bankAccount.Path });
        ValidatePageContext context = new() { CurrentNode = node, CurrentPage = bankAccount };
        
        await validator.ValidateAsync(context);

        Assert.Empty(context.ValidationResults);
    }
    
    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.NotNull(result.MemberNames.FirstOrDefault(p => p == property));
    }
}