using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Navigators;

public interface INextPageNavigator
{
    ValueTask<PagePath?> NavigateNextAsync(PageModel page);
}