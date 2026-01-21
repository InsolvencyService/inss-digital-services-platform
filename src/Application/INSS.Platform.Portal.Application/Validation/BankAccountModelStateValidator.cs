using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.Portal.Application.Validation;

public sealed class BankAccountModelStateValidator : IModelStateValidator<BankAccountModel>
{
    private readonly IBankClient _bankClient;

    public BankAccountModelStateValidator(IBankClient bankClient)
    {
        _bankClient = bankClient;
    }
    
    public async Task ValidateAsync(ModelStateDictionary modelState, BaseModel model)
    {
        if (model is not BankAccountModel bankAccount)
        {
            throw new InvalidCastException("Unable to cast the model to the bank account model.");
        }

        BankAccountVerificationRequest request = new() { BankAccount = bankAccount.AccountNumber, SortCode = bankAccount.SortCode };
        BankAccountVerificationResponse response = await this._bankClient.VerifyBankDetailsAsync(request);

        if (!response.Result)
        {
            modelState.AddModelError(nameof(bankAccount.AccountNumber), "Bank account account number not found");
            modelState.AddModelError(nameof(bankAccount.SortCode), "Bank account sort code not found");
        }
    }
}