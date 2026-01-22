using INSS.Platform.Portal.Domain.Validation;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

public class BankDetailsModel : FormBase
{
    [Required(ErrorMessage = "Enter the name on the account")]
    public string AccountName { get; set; }

    [Required(ErrorMessage = "Enter a sort code")]
    [RegularExpression(@"^(\d{2}[- ]?\d{2}[- ]?\d{2})$", ErrorMessage = "Enter a valid sort code like 309430")]
    public string SortCode
    {
        get => field?.Replace("-", string.Empty).Replace(" ", string.Empty) ?? string.Empty;
        set;
    } = string.Empty;

    [Required(ErrorMessage = "Enter an account number")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "Account number must be between 6 and 8 digits")]
    public string AccountNumber { get; init; }

    [BuildingSocietyRollNumber]
    public string? BuildingSocietyRollNumber { get; init; } = string.Empty;
}

