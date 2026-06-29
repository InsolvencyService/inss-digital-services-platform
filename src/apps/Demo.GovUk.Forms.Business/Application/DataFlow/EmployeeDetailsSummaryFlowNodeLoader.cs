using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.Business.Application.DataFlow;

public sealed class EmployeeDetailsSummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        AddAnotherModel addAnother = context.Section.Pages.GetFirstOf<AddAnotherModel>();
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        context.Section.ReturnUrl = summary.Path;
        
        List<SummaryCategoryDetail> details = [];

        foreach (PageModel page in addAnother.Items)
        {
            if (page is FullNameModel fullName)
            {
                AppendSummaryDetail(details, "Employee name", [fullName.Value], details.Count == 0 ? addAnother.Path : null);
            }
            else if (page is AgeModel age)
            {
                AppendSummaryDetail(details, "Employee age", [age.Value.AsString()], details.Count == 0 ? addAnother.Path : null);
            }
        }

        SummaryCategory employeeDetailsCategory = new() { Label = "Employee details", Details = details.ToArray() };

        summary.Categories = [employeeDetailsCategory];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}