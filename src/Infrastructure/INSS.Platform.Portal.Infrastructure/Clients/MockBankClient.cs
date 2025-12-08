using INSS.Platform.Portal.Application.Clients;

namespace INSS.Platform.Portal.Infrastructure.Clients;

public sealed class MockBankClient : IBankClient
{
    public Task<bool> BankAccountExistsAsync(string accountNumber, string sortCode)
    {
        // Specific details that will be invalid
        bool isInvalid = accountNumber == "12345678" && sortCode == "99-99-99";
        return Task.FromResult(!isInvalid);
    }
}