using System.Collections.Concurrent;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Clients;

public sealed class MockUploadContentBlobClient : IUploadContentBlobClient
{
    private static readonly ConcurrentDictionary<string, string> _cache = new();
    
    public Task<string> GetAsync(string sessionId)
    {
        return _cache.TryGetValue(sessionId, out string? xml) 
            ? Task.FromResult(xml) 
            : throw new InvalidOperationException($"Unable to find the XML for the session {sessionId}");
    }
    public Task SaveAsync(string xml, string sessionId)
    {
        _cache[sessionId] = xml;
        return Task.CompletedTask;
    }
}