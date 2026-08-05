using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain;

public sealed class PageModelList : List<PageModel>
{
    public PageModel Get(PagePath path)
    {
        PageModel? page = this.FirstOrDefault(p => p.Path == path);
        return page ?? throw new ComponentException($"Cannot get page for path {page}.");
    }

    public PageModel GetFirstPageAssociatedTo<TComponent>() where TComponent : ComponentModel
    {
        foreach (PageModel page in this)
        {
            if (page.Components.HasComponent<TComponent>())
            {
                return page;
            }
        }

        throw new ComponentException($"Cannot get page for component {typeof(TComponent).Name}.");
    }
}