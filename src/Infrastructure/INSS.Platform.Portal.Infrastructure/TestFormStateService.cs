using System.Collections.Concurrent;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Infrastructure;

public sealed class TestFormStateService : IFormStateService
{
    private static readonly ConcurrentDictionary<string, FormModel> _cache = new();

    public Task<bool> FormExistsAsync(string sessionId)
    {
        return Task.FromResult(_cache.ContainsKey(sessionId));
    }
    
    public Task<FormModel> GetAsync(string sessionId)
    {
        if (_cache.TryGetValue(sessionId, out var model))
        {
            return Task.FromResult(model);
        }

        throw new InvalidOperationException($"Unable to find the form model for the session {sessionId}");
    }

    public Task SaveAsync(string sessionId, FormModel model)
    {
        _cache[sessionId] = model;
        return Task.CompletedTask;
    }
}