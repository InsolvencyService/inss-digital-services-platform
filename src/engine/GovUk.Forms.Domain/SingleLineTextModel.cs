using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public sealed class SingleLineTextModel : PageModel
{
    [Required(ErrorMessage = "Enter text")]
    public string Value { get; set; } = string.Empty;

    public override string[] GetSummaryInfo()
    {
        return [Value];
    }

    public override void CopyTo(PageModel target)
    {
        SingleLineTextModel singleLineText = target.As<SingleLineTextModel>();
        singleLineText.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = string.Empty;
    }
}