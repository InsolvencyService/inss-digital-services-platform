using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;

namespace INSS.Platform.Portal.Infrastructure.Clients;

public sealed class MockBankClient : IBankClient
{
    public Task<BankAccountVerificationResponse> VerifyBankDetailsAsync(BankAccountVerificationRequest request)
    {
        bool isInvalid = request.BankAccount == "12345678" && request.SortCode == "99-99-99";
        return Task.FromResult(new BankAccountVerificationResponse() { Result = isInvalid });
    }
}