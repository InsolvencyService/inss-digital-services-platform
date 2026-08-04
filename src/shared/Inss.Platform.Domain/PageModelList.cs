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
}