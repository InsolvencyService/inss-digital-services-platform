using Inss.Platform.Domain.Components.Searching;

namespace Inss.Platform.Application.Loaders;

public sealed class SearchTermComponentLoader : IComponentLoader
{
    public ValueTask LoadAsync(LoaderContext context)
    {
        SearchTermComponentModel searchTerm = context.Component.As<SearchTermComponentModel>();
        searchTerm.Value = null;
        return ValueTask.CompletedTask;
    }
}