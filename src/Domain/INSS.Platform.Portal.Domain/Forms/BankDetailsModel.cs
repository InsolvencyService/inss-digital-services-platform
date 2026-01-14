using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

public class BankDetailsModel : FormBase
{
    [Required(ErrorMessage = "Enter your account number")]
    public string AccountNumber { get; init; }

    [Required(ErrorMessage = "Enter your sourt code")]
    public string SortCode { get; init; } = string.Empty;
}

