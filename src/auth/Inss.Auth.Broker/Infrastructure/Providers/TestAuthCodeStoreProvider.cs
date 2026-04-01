using System.Collections.Concurrent;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;

namespace Inss.Auth.Broker.Infrastructure.Providers;

public sealed class TestAuthCodeStoreProvider : IAuthCodeStoreProvider
{
    private static readonly ConcurrentDictionary<string, AuthCode> _cache = new();
    
    public Task StoreAsync(string code, AuthCode user)
    {
        _cache[code] = user;
        return Task.CompletedTask;
    }

    public Task<AuthCode?> GetAsync(string code)
    {
        return Task.FromResult<AuthCode?>(_cache[code]);
    }

    public Task RemoveAsync(string code)
    {
        _ = _cache.TryRemove(code, out _);
        return Task.CompletedTask;
    }
}