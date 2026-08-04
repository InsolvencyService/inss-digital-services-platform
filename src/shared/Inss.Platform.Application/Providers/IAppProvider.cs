using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Providers;

public interface IAppProvider
{
    ValueTask<bool> ExistsAsync(SessionId session);
    ValueTask<AppModel> GetAsync(SessionId session);
    ValueTask SaveAsync(SessionId session, AppModel app);
    ValueTask RemoveAsync(SessionId session, AppModel app);
}