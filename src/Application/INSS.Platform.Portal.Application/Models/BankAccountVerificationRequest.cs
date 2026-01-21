namespace INSS.Platform.Portal.Application.Models;

/// <summary>
/// Represents a request to verify a bank account, including customer and account details.
/// </summary>
public class BankAccountVerificationRequest
{
    /// <summary>
    /// Gets or sets the name of the customer whose bank account is to be verified.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort code of the bank account.
    /// </summary>
    public string SortCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank account number.
    /// </summary>
    public string BankAccount { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the bank account.
    /// </summary>
    public AccountType AccountType { get; set; }
}
