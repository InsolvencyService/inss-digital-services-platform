using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class EmployerDetailsModel : PageModel
{
    public string CaseReference { get; set; }
    
    public string EmployerName { get; set; }

    public bool DetailsMatch { get; set; }

    public override string[] GetSummaryInfo()
    {
        return [EmployerName];
    }

    public override void CopyTo(PageModel target)
    {
        EmployerDetailsModel employerDetails = target.As<EmployerDetailsModel>();
        employerDetails.CaseReference = CaseReference;
        employerDetails.EmployerName = EmployerName;
        employerDetails.DetailsMatch = DetailsMatch;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        CaseReference = string.Empty;
        EmployerName = string.Empty;
        DetailsMatch = false;
    }
}