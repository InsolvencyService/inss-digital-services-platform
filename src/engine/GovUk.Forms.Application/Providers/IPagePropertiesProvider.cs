using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Providers;

public interface IPagePropertiesProvider
{
    ContentPath? PreviousPagePath { get; set; }
    bool FullPageLayout { get; set; }
}