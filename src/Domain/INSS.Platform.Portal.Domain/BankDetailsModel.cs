using INSS.Platform.Portal.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents the bank details form model, including account name, sort code, account number, and optional building society roll number.
/// </summary>
public class BankDetailsModel : FormBase
{
    /// <summary>
    /// Gets or sets the name on the bank account.
    /// </summary>
    /// <remarks>
    /// This field is required.
    /// </remarks>
    [Required(ErrorMessage = "Enter the name on the account")]
    public string AccountName { get; set; }

    /// <summary>
    /// Gets or sets the sort code for the bank account.
    /// </summary>
    /// <remarks>
    /// This field is required and must match the format of a UK sort code (e.g., 309430).
    /// Dashes and spaces are removed from the value.
    /// </remarks>
    [Required(ErrorMessage = "Enter a sort code")]
    [RegularExpression(@"^(\d{2}[- ]?\d{2}[- ]?\d{2})$", ErrorMessage = "Enter a valid sort code like 309430")]
    public string SortCode
    {
        get => field?.Replace("-", string.Empty).Replace(" ", string.Empty) ?? string.Empty;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets the account number for the bank account.
    /// </summary>
    /// <remarks>
    /// This field is required and must be exactly 8 digits.
    /// </remarks>
    [Required(ErrorMessage = "Enter an account number")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "Account number must be between 6 and 8 digits")]
    public string AccountNumber { get; init; }

    /// <summary>
    /// Gets the building society roll number, if applicable.
    /// </summary>
    /// <remarks>
    /// This field is optional and validated by the <see cref="BuildingSocietyRollNumberAttribute"/>.
    /// </remarks>
    [BuildingSocietyRollNumber]
    public string? BuildingSocietyRollNumber { get; init; } = string.Empty;
}

