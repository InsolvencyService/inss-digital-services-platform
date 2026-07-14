using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class PagePropertiesProvider : IPagePropertiesProvider
{
    public ContentPath? PreviousPagePath { get; set; }
    
    public bool FullPageLayout { get; set; }
    
    public string PageTitle { get; set; }
}