using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Validation;

public class DefaultFlowNodeValidatorTests
{
    [Fact]
    public async Task InvalidBankAccount_ValidateAsync_ReturnsError()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.AccountName = "H J Simpson";
        bankAccount.SortCode = "112233";
        bankAccount.AccountNumber = "12345678";
        IFlowNodeValidator validator = DefaultFlowNodeValidator.Default;
        FlowNode node = new() { Id = "NodeId1", PagePath = bankAccount.Path };
        ValidateContext context = new() { Nodes = [node], CurrentNode = node, Page = bankAccount };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Single(validationResults);
        AssertError(validationResults[0], "The sort code must be in the format 11-22-33", nameof(bankAccount.SortCode));
    }
    
    [Fact]
    public async Task ValidBankAccount_ValidateAsync_ReturnsNoErrors()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.AccountName = "H J Simpson";
        bankAccount.SortCode = "11-22-33";
        bankAccount.AccountNumber = "12345678";
        IFlowNodeValidator validator = DefaultFlowNodeValidator.Default;
        FlowNode node = new() { Id = "NodeId1", PagePath = bankAccount.Path };
        ValidateContext context = new() { Nodes = [node], CurrentNode = node, Page = bankAccount };
        
        ValidationResult[] validationResults = await validator.ValidateAsync(context);

        Assert.Empty(validationResults);
    }

    private static void AssertError(ValidationResult result, string message, string property)
    {
        Assert.Equal(message, result.ErrorMessage);
        Assert.Equal(property, result.MemberNames.First());
    }
}