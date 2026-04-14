using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GovUk.Forms.Domain;

public sealed class SalaryModel : PageModel
{
    [Required(ErrorMessage = "You must enter a salary")]
    [Range(1_000, 1_000_000, ErrorMessage = "The salary is not between £1,000 and £1,000,000 inclusive")]
    public int Value { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [Value.ToString("C", CultureInfo.CurrentCulture)];
    }
    
    public override void CopyTo(PageModel target)
    {
        SalaryModel salary = target.As<SalaryModel>();
        salary.Value = Value;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Value = 0;
    }
}