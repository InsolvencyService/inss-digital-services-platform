using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Application.PageFlow;

[ExcludeFromCodeCoverage]
public sealed class NoopPageLoader : IPageLoader
{
    public static readonly IPageLoader Default = new NoopPageLoader();
    
    private NoopPageLoader()
    {
    }
    
    public ValueTask LoadAsync(LoadPageContext context)
    {
        return ValueTask.CompletedTask;
    }
}