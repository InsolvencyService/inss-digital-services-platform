using System.Collections.Concurrent;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Infrastructure.Providers;

// TODO: Move?
public sealed class TempAppProvider : IAppProvider
{
    private static readonly ConcurrentDictionary<SessionId, App> _cache = new();

    public ValueTask<bool> ExistsAsync(SessionId session)
    {
        return ValueTask.FromResult(_cache.ContainsKey(session));
    }
    
    public ValueTask<App> GetAsync(SessionId session)
    {
        return _cache.TryGetValue(session, out App? appPages) 
            ? ValueTask.FromResult(appPages) 
            : throw new InvalidOperationException($"Unable to find the app pages for the session {session}");
    }
    
    public ValueTask SaveAsync(SessionId session, App appPages)
    {
        _cache[session] = appPages;
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(SessionId session, App appPages)
    {
        _cache.TryRemove(session, out App? _);
        return ValueTask.CompletedTask;
    }
}