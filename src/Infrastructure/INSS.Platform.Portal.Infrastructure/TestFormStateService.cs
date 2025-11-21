using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace INSS.Platform.Portal.Infrastructure;

public sealed class TestFormStateService : IFormStateService, IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public Task<bool> FormExistsAsync(string sessionId)
    {
        return Task.FromResult(this._cache.TryGetValue(sessionId, out _));
    }
    
    public Task<FormModel> GetAsync(string sessionId)
    {
        if (_cache.TryGetValue(sessionId, out var model))
        {
            return Task.FromResult((FormModel)model!);
        }

        throw new InvalidOperationException($"Unable to find the form model for the session {sessionId}");
    }

    public Task SaveAsync(string sessionId, FormModel model)
    {
        _cache.Set(sessionId, model);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}