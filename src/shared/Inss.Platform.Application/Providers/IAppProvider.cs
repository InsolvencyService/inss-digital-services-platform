using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Providers;

public interface IAppProvider
{
    ValueTask<bool> ExistsAsync(SessionId session);
    ValueTask<App> GetAsync(SessionId session);
    ValueTask SaveAsync(SessionId session, App appPages);
    ValueTask RemoveAsync(SessionId session, App appPages);
}