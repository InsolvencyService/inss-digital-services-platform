using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class OwnHomePageExecutor : IPageExecutor
{
    private const int OwnHomeNodeIndex = 0;
    private const int NotOwnHomeNodeIndex = 1;
    
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        OwnHomeModel ownHome = context.CurrentPage.As<OwnHomeModel>();
        context.ChildNodeIndex = ownHome.OwnsHome ? OwnHomeNodeIndex : NotOwnHomeNodeIndex;
        return ValueTask.CompletedTask;
    }
}