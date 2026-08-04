using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Services;

public interface IPageService
{
    ValueTask<PageModel> LoadAsync(PagePath path, Dictionary<string, string?> queryParams);
    ValueTask<PageModel?> ValidateAsync(PageModel page);
    ValueTask<PagePath?> SaveAsync(PageModel page);
}