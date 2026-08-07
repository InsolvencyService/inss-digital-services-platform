using System.Collections.Concurrent;
using Inss.Platform.RpsProvider.Application.Providers;
using Inss.Platform.RpsProvider.Domain;

namespace Inss.Platform.RpsProvider.Infrastructure.Providers;

public sealed class TestUserAuthStoreProvider : IUserAuthStoreProvider
{
    private static readonly ConcurrentDictionary<string, UserAuth> _cache = new();
    
    public Task StoreAsync(UserAuth user)
    {
        _cache[user.Id] = user;
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