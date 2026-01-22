using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Validation;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Xunit;

namespace INSS.Platform.Portal.Application.Tests.Validation;

public class BankAccountModelStateValidatorTests
{
    private readonly BankAccountModelStateValidator _validator;
    private readonly Mock<IBankClient> _mockBankClient;
    private readonly ModelStateDictionary _modelState;

    public BankAccountModelStateValidatorTests()
    {
        _mockBankClient = new Mock<IBankClient>();
        _validator = new BankAccountModelStateValidator(_mockBankClient.Object);
        _modelState = new ModelStateDictionary();
    }

    [Fact]
    public async Task InvalidMode_ValidateAsync_NoModelErrors()
    {
        BankAccountModel bankAccountModel = new() { AccountNumber = "12345678", SortCode = "12-34-56" };
        _mockBankClient
            .Setup(client => client.VerifyBankDetailsAsync(It.Is<Models.BankAccountVerificationRequest>(
                req => req.BankAccount == "12345678" && req.SortCode == "12-34-56")))
            .ReturnsAsync(new Models.BankAccountVerificationResponse() { Result = false });

        await _validator.ValidateAsync(_modelState, bankAccountModel);

        Assert.False(_modelState.IsValid);
    }

    [Fact]
    public async Task ValidModel_ValidateAsync_NoModelErrors()
    {
        BankAccountModel bankAccountModel = new() { AccountNumber = "12345678", SortCode = "12-34-56"};

        _mockBankClient
            .Setup(client => client.VerifyBankDetailsAsync(It.Is<Models.BankAccountVerificationRequest>(
                req => req.BankAccount == "12345678" && req.SortCode == "12-34-56")))
            .ReturnsAsync(new Models.BankAccountVerificationResponse() { Result = true });

        await _validator.ValidateAsync(_modelState, bankAccountModel);

        Assert.True(_modelState.IsValid);
    }
}
