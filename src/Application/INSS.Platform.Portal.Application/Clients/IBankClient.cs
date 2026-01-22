using INSS.Platform.Portal.Application.Models;

namespace INSS.Platform.Portal.Application.Clients;

/// <summary>
/// Defines methods for interacting with bank-related services.
/// </summary>
public interface IBankClient
{
    /// <summary>
    /// Verifies the provided bank account details asynchronously.
    /// </summary>
    /// <param name="request">The bank account verification request containing account details to verify.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a <see cref="BankAccountVerificationResponse"/>
    /// indicating the outcome of the verification.
    /// </returns>
    Task<BankAccountVerificationResponse> VerifyBankDetailsAsync(BankAccountVerificationRequest request);
}