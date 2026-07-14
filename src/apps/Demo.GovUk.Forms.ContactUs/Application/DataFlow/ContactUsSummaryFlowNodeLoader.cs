using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.ContactUs.Application.DataFlow;

public sealed class ContactUsSummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        FullNameModel fullName = context.Section.Pages.GetFirstOf<FullNameModel>();
        AddAnotherModel addAnother = context.Section.Pages.GetFirstOf<AddAnotherModel>();
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        context.Section.ReturnUrl = summary.Path;
        
        List<SummaryCategoryDetail> details = [];

        AppendSummaryDetail(details, "Contact name", [fullName.Value], details.Count == 0 ? addAnother.Path : null);
        
        foreach (PageModel page in addAnother.Items)
        {
            if (page is FileUploadModel fileUpload)
            {
                AppendSummaryDetail(details, "Attachment", [fileUpload.Filename], details.Count == 0 ? addAnother.Path : null);
            }
        }

        SummaryCategory contactCategoryDetails = new() { Label = "Contact details", Details = details.ToArray() };

        summary.Categories = [contactCategoryDetails];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}