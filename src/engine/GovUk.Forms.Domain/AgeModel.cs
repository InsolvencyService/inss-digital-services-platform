using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GovUk.Forms.Domain;

public sealed class AgeModel : PageModel
{
    [Required(ErrorMessage = "You must enter an age")]
    [Range(16, 80, ErrorMessage = "The age is not between 16 and 80 inclusive")]
    public int Value { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [Value.ToString(CultureInfo.InvariantCulture)];
    }

    public override void CopyTo(PageModel target)
    {
        AgeModel age = target.As<AgeModel>();
        age.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = 0;
    }
}