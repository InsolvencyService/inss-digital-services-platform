using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class IPUploadSummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        CheckCaseReferenceModel caseReference = context.Section.Pages.GetFirstOf<CheckCaseReferenceModel>();
        EmployerDetailsModel employerDetails = context.Section.Pages.GetFirstOf<EmployerDetailsModel>();
        XmlFileUploadModel fileUpload = context.Section.Pages.GetFirstOf<XmlFileUploadModel>();
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        SetReturnUrl(summary, context.Section.Pages.GetCompletedPages());
        
        List<SummaryCategoryDetail> details = [];
        
        AppendSummaryDetail(details, "Enter the 10 character case reference number", [employerDetails.CaseReference], caseReference.Path);
        AppendSummaryDetail(details, "Employer name", [employerDetails.EmployerName]);
        AppendSummaryDetail(details, "Is this the correct employer name?", [employerDetails.DetailsMatch ? "Yes" : "No"]);
        
        SummaryCategory caseInfoCategory = new() { Label = "We have matched to the following employer", Details = details.ToArray() };

        details = [];
        
        AppendSummaryDetail(details, "Uploaded document", [fileUpload.Filename]);
        
        SummaryCategory fileInfoCategory = new() { Label = "Your form has passed initial validation.", Details = details.ToArray() };

        summary.Categories = [caseInfoCategory, fileInfoCategory];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}