using Inss.Platform.Domain;

namespace Inss.Platform.Application.Validation;

public sealed class PageContext
{
    public PageContext(PageModel page)
    {
        Page = page;
    }
    
    public PageModel Page { get; }
}