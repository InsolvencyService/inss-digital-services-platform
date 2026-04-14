using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Domain;

public sealed class OwnHomeModel : PageModel
{
    public bool OwnsHome { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        return [OwnsHome ? "Yes" : "No"];
    }
    
    public override void CopyTo(PageModel target)
    {
        OwnHomeModel ownHome = target.As<OwnHomeModel>();
        ownHome.OwnsHome = OwnsHome;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        OwnsHome = false;
    }
}