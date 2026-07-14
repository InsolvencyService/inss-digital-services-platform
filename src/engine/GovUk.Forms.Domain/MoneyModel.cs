using System.ComponentModel.DataAnnotations;
using System.Globalization;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace GovUk.Forms.Domain;

public sealed class MoneyModel : PageModel
{
    [Required(ErrorMessage = "You must enter a monetary value")]
    public int Amount { get; set; }

    public override string[] GetSummaryInfo()
    {
        return [Amount.ToString("C", CultureInfo.InvariantCulture)];
    }
    
    public override void CopyTo(PageModel target)
    {
        MoneyModel money = target.As<MoneyModel>();
        money.Amount = Amount;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Amount = 0;
    }
}