using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public sealed class FullNameModel : PageModel
{
    [Required(ErrorMessage = "Enter your full name")]
    [RegularExpression(@"^[A-Za-z\s\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens and apostrophes")]
    public string Value { get; set; } = string.Empty;
    
    public override string[] GetSummaryInfo()
    {
        return [Value];
    }

    public override void CopyTo(PageModel target)
    {
        FullNameModel fullName = target.As<FullNameModel>();
        fullName.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = string.Empty;
    }
}