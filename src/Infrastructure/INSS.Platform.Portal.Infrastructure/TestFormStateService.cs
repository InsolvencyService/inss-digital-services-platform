using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace INSS.Platform.Portal.Infrastructure;

public sealed class TestFormStateService : IFormStateService
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    
    public Task<FormModel?> GetAsync(string sessionId)
    {
        if (_cache.TryGetValue(sessionId, out var model))
        {
            return Task.FromResult((FormModel?)model);
        }

        return Task.FromResult<FormModel?>(null);
    }

    public Task SaveAsync(string sessionId, FormModel model)
    {
        _cache.Set(sessionId, model);
        return Task.CompletedTask;
    }
}