using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.PageFlow;

public sealed class YourSalaryPageExecutor : IPageExecutor
{
    private const int EqualToOrAbove10000NodeIndex = 0;
    private const int Below10000NodeIndex = 1;
    
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        SalaryModel salary = context.CurrentPage.As<SalaryModel>();
        context.ChildNodeIndex = salary.Value >= 10_000 ? EqualToOrAbove10000NodeIndex : Below10000NodeIndex;
        return ValueTask.CompletedTask;
    }
}