using System.Collections.Concurrent;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Domain;

namespace Inss.Auth.RpsProvider.Infrastructure.Providers;

public sealed class TestUserAuthStoreProvider : IUserAuthStoreProvider
{
    private static readonly ConcurrentDictionary<string, UserAuth> _cache = new();
    
    public Task StoreAsync(string code, UserAuth user)
    {
        _cache[code] = user;
        return Task.CompletedTask;
    }

    public Task<UserAuth?> GetAsync(string code)
    {
        return Task.FromResult<UserAuth?>(_cache[code]);
    }

    public Task RemoveAsync(string code)
    {
        _ = _cache.TryRemove(code, out _);
        return Task.CompletedTask;
    }
}