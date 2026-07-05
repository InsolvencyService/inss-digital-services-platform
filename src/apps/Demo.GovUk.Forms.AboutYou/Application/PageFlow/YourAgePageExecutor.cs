using GovUk.Forms.Application.PageFlow;
using GovUk.Forms.Domain;

namespace Demo.GovUk.Forms.AboutYou.Application.PageFlow;

public sealed class YourAgePageExecutor : IPageExecutor
{
    private const int EqualToOrAbove18NodeIndex = 0;
    private const int Below18NodeIndex = 1;
    
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        AgeModel age = context.CurrentPage.As<AgeModel>();
        context.ChildNodeIndex = age.Value >= 18 ? EqualToOrAbove18NodeIndex : Below18NodeIndex;
        return ValueTask.CompletedTask;
    }
}