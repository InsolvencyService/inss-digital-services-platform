using GovUk.Forms.Domain;
using System.Text.Json.Serialization;
using static GovUk.Forms.Domain.AddAnotherModel;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class EmployerDetailsModel : PageModel
{
    public PageModelList Items { get; init; } = [];
    public bool YesorNoItem { get; set; }
    public string CaseRefNum { get; set; }
    public string EmployerName { get; set; }

    [JsonIgnore]
    public AddAnotherSummaryModel[] SummaryInfo { get; set; } = [];

    public override string[] GetSummaryInfo()
    {
        return Items.SelectMany(p => p.GetSummaryInfo()).ToArray();
    }

    public override void CopyTo(PageModel target)
    {
        EmployerDetailsModel employerDetails = target.As<EmployerDetailsModel>();
        employerDetails.CaseRefNum = CaseRefNum;
        employerDetails.EmployerName = EmployerName;
        employerDetails.YesorNoItem = YesorNoItem;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        CaseRefNum = string.Empty;
        EmployerName = string.Empty;
        YesorNoItem = false;
    }
}