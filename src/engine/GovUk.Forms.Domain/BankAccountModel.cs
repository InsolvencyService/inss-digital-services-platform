using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class BankAccountModel : PageModel
{
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "Enter the name on the account")]
    [MaxLength(255, ErrorMessage = "Account name cannot exceed 255 characters")]
    public string AccountName { get; set; }
    
    [Copyable]
    [Summary]
    [Required(ErrorMessage = "You must enter an account number")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "The account number must be 8 numbers")]
    public string AccountNumber { get; set; } = string.Empty;

    [Copyable]
    [Summary]
    [Required(ErrorMessage = "You must enter a sort code")]
    [RegularExpression("^[0-9]{2}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "The sort code must be in the format 11-22-33")]
    public string SortCode { get; set; } = string.Empty;
    
    [Copyable]
    [Summary]
    public string? BuildingSocietyRollNumber { get; init; } = string.Empty;
}