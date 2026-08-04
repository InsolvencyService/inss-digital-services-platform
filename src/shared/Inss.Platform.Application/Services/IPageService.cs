using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Services;

public interface IPageService
{
    ValueTask<Page> LoadAsync(PagePath path, Dictionary<string, string?> queryParams);
    ValueTask<Page?> ValidateAsync(Page page);
    ValueTask<PagePath?> SaveAsync(Page page);
}