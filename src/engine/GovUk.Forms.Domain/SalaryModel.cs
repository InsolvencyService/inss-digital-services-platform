using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class SalaryModel : PageModel
{
    [Copyable]
    [Required(ErrorMessage = "You must enter a salary")]
    [Range(1_000, 1_000_000, ErrorMessage = "The salary is not between £1,000 and £1,000,000 inclusive")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Value { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [Value.ToString("C", CultureInfo.CurrentCulture)];
    }
}