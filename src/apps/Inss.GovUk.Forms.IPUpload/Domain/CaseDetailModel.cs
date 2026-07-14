using GovUk.Forms.Domain;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.GovUk.Forms.IPUpload.Domain;

public class CaseDetailModel : PageModel
{
    public string? CaseReference { get; set; }
    
    public string? CompanyName { get; set; }
    
    public override void ClearValues()
    {
        base.ClearValues();
        CaseReference = null;
        CompanyName = null;
    }
}