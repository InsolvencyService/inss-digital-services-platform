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
        string key = GetKey(path.GetRoot(), sessionId);
        return Task.FromResult(_cache.ContainsKey(key));
    }

    public Task<FormModel> GetAsync(ContentPath path, string sessionId)
    {
        string key = GetKey(path.GetRoot(), sessionId);
        return _cache.TryGetValue(key, out FormModel? form) 
            ? Task.FromResult(form) 
            : throw new StorageProviderException($"Unable to find the form model for the session {sessionId}");
    }
    
    public Task SaveAsync(string sessionId, FormModel form)
    {
        string key = GetKey(form.Path, sessionId);
        _cache[key] = form;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string sessionId, FormModel form)
    {
        string key = GetKey(form.Path, sessionId);
        _cache.TryRemove(key, out FormModel? _);
        return Task.CompletedTask;
    }
    
    private static string GetKey(ContentPath path, string sessionId)
    {
        return $"{path}-{sessionId}";
    }
}