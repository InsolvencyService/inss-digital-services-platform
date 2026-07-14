using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Infrastructure.Exceptions;

namespace GovUk.Forms.Infrastructure.Providers;

[ExcludeFromCodeCoverage]
public sealed class TestFormStorageProvider : IFormStorageProvider
{
    private static readonly ConcurrentDictionary<string, FormModel> _cache = new();
    
    public Task<bool> ExistsAsync(ContentPath path, string sessionId)
    {
        return Task.FromResult(_cache.ContainsKey(sessionId));
    }

    public Task<FormModel> GetAsync(ContentPath path, string sessionId)
    {
        return _cache.TryGetValue(sessionId, out FormModel? form) 
            ? Task.FromResult(form) 
            : throw new StorageProviderException($"Unable to find the form model for the session {sessionId}");
    }
    
    public Task SaveAsync(string sessionId, FormModel form)
    {
        _cache[sessionId] = form;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string sessionId, FormModel form)
    {
        _cache.TryRemove(sessionId, out FormModel? _);
        return Task.CompletedTask;
    }
}