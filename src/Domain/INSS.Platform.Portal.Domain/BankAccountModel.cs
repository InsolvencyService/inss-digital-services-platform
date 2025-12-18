using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class BankAccountModel : BaseModel
{
    public BankAccountModel()
    {
        PathName = "bank-account";
        Title = "Bank Account";
    }
    
    [Required(ErrorMessage = "You must enter an account number")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "The account number must be 8 numbers")]
    public string AccountNumber { get; init; } = string.Empty;

    [Required(ErrorMessage = "You must enter a sort code")]
    [RegularExpression("^[0-9]{2}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "The sort code must be in the format 11-22-33")]
    public string SortCode { get; init; } = string.Empty;
}