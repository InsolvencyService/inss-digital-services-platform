namespace INSS.Platform.Portal.Application.Models;

/// <summary>
/// Represents the response from a bank account verification process.
/// </summary>
public class BankAccountVerificationResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the bank account verification was successful.
    /// </summary>
    public bool Result { get; set; }

    /// <summary>
    /// Gets or sets the descriptive text for the verification result.
    /// </summary>
    public string ResultText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason code associated with the verification result.
    /// </summary>
    public string ReasonCode { get; set; } = string.Empty;
}
