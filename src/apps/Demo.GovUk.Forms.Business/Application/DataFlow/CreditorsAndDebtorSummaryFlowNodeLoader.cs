using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.Business.Application.DataFlow;

public sealed class CreditorsAndDebtorSummaryFlowNodeLoader : SummaryFlowNodeLoader
{
    public override ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        AddAnotherModel addAnotherCreditor = context.Section.Pages.GetAtIndex<AddAnotherModel>(0);
        AddAnotherModel addAnotherDebtor = context.Section.Pages.GetAtIndex<AddAnotherModel>(1);
        SummaryModel summary = context.Section.Pages.GetFirstOf<SummaryModel>();

        context.Section.ReturnUrl = summary.Path;
        
        List<SummaryCategoryDetail> details = [];

        foreach (PageModel page in addAnotherCreditor.Items)
        {
            if (page is FullNameModel fullName)
            {
                AppendSummaryDetail(details, "Creditor name", [fullName.Value], details.Count == 0 ? addAnotherCreditor.Path : null);
            }
            else if (page is MoneyModel money)
            {
                AppendSummaryDetail(details, "Creditor amount", [money.Amount.AsMoney()], details.Count == 0 ? addAnotherCreditor.Path : null);
            }
        }

        SummaryCategory creditorCategoryDetails = new() { Label = "Creditor details", Details = details.ToArray() };

        details = [];

        foreach (PageModel page in addAnotherDebtor.Items)
        {
            if (page is FullNameModel fullName)
            {
                AppendSummaryDetail(details, "Debtor name", [fullName.Value], details.Count == 0 ? addAnotherCreditor.Path : null);
            }
            else if (page is MoneyModel money)
            {
                AppendSummaryDetail(details, "Debtor amount", [money.Amount.AsMoney()], details.Count == 0 ? addAnotherCreditor.Path : null);
            }
        }

        SummaryCategory debtorCategoryDetails = new() { Label = "Debtor details", Details = details.ToArray() };
        
        summary.Categories = [creditorCategoryDetails, debtorCategoryDetails];
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}