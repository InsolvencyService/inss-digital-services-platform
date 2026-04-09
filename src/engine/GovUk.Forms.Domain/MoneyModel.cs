using System.ComponentModel.DataAnnotations;
using System.Globalization;
using GovUk.Forms.Domain.Attributes;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace GovUk.Forms.Domain;

public sealed class MoneyModel : PageModel
{
    [Copyable]
    [Required(ErrorMessage = "You must enter a monetary value")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public int Amount { get; set; }

    public override string[] GetSummaryInfo()
    {
        return [Amount.ToString("C", CultureInfo.CurrentCulture)];
    }
}