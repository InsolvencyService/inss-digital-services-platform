using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Application.PageFlow;

[ExcludeFromCodeCoverage]
public sealed class NoopPageExecutor : IPageExecutor
{
    public static readonly IPageExecutor Default = new NoopPageExecutor();
    
    private NoopPageExecutor()
    {
    }
    
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        context.ChildNodeIndex = 0;
        return ValueTask.CompletedTask;
    }
}