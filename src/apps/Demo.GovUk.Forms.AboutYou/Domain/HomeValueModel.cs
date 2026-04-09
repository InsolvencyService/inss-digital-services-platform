using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public class HomeValueModel : PageModel
{
    [Copyable]
    [Required(ErrorMessage = "Enter your home value")]
    [Range(100, 1_000_000, ErrorMessage = "The value must between £100 and £1,000,000")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Value { get; set; }

    public override string[] GetSummaryInfo()
    {
        return [Value.ToString("C", CultureInfo.CurrentCulture)];
    }
}