using Inss.Platform.Domain;

namespace Inss.Platform.Application.Providers;

public interface IAppProvider
{
    ValueTask<AppModel> GetAsync();
    ValueTask SaveAsync(AppModel app);
    ValueTask RemoveAsync(AppModel app);
}