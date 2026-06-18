using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public sealed class RequiredTextModel : PageModel
{
    [Required]
    public string Value { get; set; } = string.Empty;
    
    public override string[] GetSummaryInfo()
    {
        return [Value];
    }

    public override void CopyTo(PageModel target)
    {
        RequiredTextModel fullName = target.As<RequiredTextModel>();
        fullName.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = string.Empty;
    }
}