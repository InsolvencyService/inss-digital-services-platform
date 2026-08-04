using Inss.Platform.Application.Exceptions;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Navigators;

public sealed class DefaultNextPageNavigator : INextPageNavigator
{
    public ValueTask<PagePath?> NavigateNextAsync(Page page)
    {
        return page.NextPages.Count switch
        {
            0 => ValueTask.FromResult<PagePath?>(null),
            1 => ValueTask.FromResult<PagePath?>(page.NextPages[0]),
            _ => throw new NextPageException("Cannot handle multiple next pages from the default next navigator.")
        };
    }
}