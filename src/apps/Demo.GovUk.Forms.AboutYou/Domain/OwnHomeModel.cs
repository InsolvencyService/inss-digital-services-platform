using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Attributes;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public sealed class OwnHomeModel : PageModel
{
    [Copyable]
    public bool OwnsHome { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [OwnsHome ? "Yes" : "No"];
    }
}