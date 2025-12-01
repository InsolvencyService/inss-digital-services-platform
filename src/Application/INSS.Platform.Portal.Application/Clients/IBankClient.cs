namespace INSS.Platform.Portal.Application.Clients;

public interface IBankClient
{
    Task<bool> BankAccountExistsAsync(string accountNumber, string sortCode);
}