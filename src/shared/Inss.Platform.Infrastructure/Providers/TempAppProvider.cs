using System.Collections.Concurrent;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Infrastructure.Providers;

// TODO: Move?
public sealed class TempAppProvider : IAppProvider
{
    private static readonly ConcurrentDictionary<SessionId, AppModel> _cache = new();

    public ValueTask<bool> ExistsAsync(SessionId session)
    {
        return ValueTask.FromResult(_cache.ContainsKey(session));
    }
    
    public ValueTask<AppModel> GetAsync(SessionId session)
    {
        return _cache.TryGetValue(session, out AppModel? app) 
            ? ValueTask.FromResult(app) 
            : throw new InvalidOperationException($"Unable to find the app pages for the session {session}");
    }
    
    public ValueTask SaveAsync(SessionId session, AppModel app)
    {
        _cache[session] = app;
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(SessionId session, AppModel app)
    {
        _cache.TryRemove(session, out AppModel? _);
        return ValueTask.CompletedTask;
    }
}