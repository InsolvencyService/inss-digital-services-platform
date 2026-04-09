using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public class HomeValueModel : PageModel
{
    [Required(ErrorMessage = "Enter your home value")]
    [Range(100, 1_000_000, ErrorMessage = "The value must between £100 and £1,000,000")]
    public int Value { get; set; }

    public override string[] GetSummaryInfo()
    {
        return [Value.ToString("C", CultureInfo.CurrentCulture)];
    }

    public override void CopyTo(PageModel target)
    {
        HomeValueModel homeValue = target.As<HomeValueModel>();
        homeValue.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = 0;
    }
}