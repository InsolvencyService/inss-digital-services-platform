using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain.Attributes;

namespace GovUk.Forms.Domain;

public sealed class AgeModel : PageModel
{
    [Copyable]
    [Required(ErrorMessage = "You must enter an age")]
    [Range(16, 80, ErrorMessage = "The age is not between 16 and 80 inclusive")]
    public int Value { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [Value.ToString(CultureInfo.CurrentCulture)];
    }
}