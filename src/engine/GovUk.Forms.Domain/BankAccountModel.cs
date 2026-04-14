using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public sealed class BankAccountModel : PageModel
{
    [Required(ErrorMessage = "Enter the name on the account")]
    [MaxLength(255, ErrorMessage = "Account name cannot exceed 255 characters")]
    public string AccountName { get; set; }
    
    [Required(ErrorMessage = "You must enter an account number")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage = "The account number must be 8 numbers")]
    public string AccountNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "You must enter a sort code")]
    [RegularExpression("^[0-9]{2}-[0-9]{2}-[0-9]{2}$", ErrorMessage = "The sort code must be in the format 11-22-33")]
    public string SortCode { get; set; } = string.Empty;
    
    public string? BuildingSocietyRollNumber { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [AccountName, AccountNumber,  SortCode, BuildingSocietyRollNumber!];
    }

    public override void CopyTo(PageModel target)
    {
        BankAccountModel bankAccount = target.As<BankAccountModel>();
        bankAccount.AccountName = AccountName;
        bankAccount.AccountNumber = AccountNumber;
        bankAccount.SortCode = SortCode;
        bankAccount.BuildingSocietyRollNumber = BuildingSocietyRollNumber;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        AccountName = string.Empty;
        AccountNumber = string.Empty;
        SortCode = string.Empty;
        BuildingSocietyRollNumber = null;
    }
}