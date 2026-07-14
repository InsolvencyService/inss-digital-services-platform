using System.Collections.Concurrent;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;

namespace Inss.Auth.Broker.Infrastructure.Providers;

public sealed class TestAuthCodeStoreProvider : IAuthCodeStoreProvider
{
    private static readonly ConcurrentDictionary<string, AuthCode> _cache = new();
    
    public Task StoreAsync(AuthCode authCode)
    {
        _cache[authCode.Id] = authCode;
        return Task.CompletedTask;
    }

    public Task<AuthCode?> GetAsync(string id)
    {
        return Task.FromResult<AuthCode?>(_cache[id]);
    }

    public Task RemoveAsync(string id)
    {
        _ = _cache.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}