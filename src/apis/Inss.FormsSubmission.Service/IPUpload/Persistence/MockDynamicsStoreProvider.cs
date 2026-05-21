using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

[ExcludeFromCodeCoverage]
public sealed class MockDynamicsStoreProvider : IDynamicsStoreProvider
{
    private static readonly ConcurrentDictionary<string, DynamicsSubmission> _cache = new();
    
    public Task StoreAsync(DynamicsSubmission submission, CancellationToken cancellationToken)
    {
        _cache[submission.Id] = submission;
        return Task.CompletedTask;
    }

    public Task<DynamicsSubmission?> GetAsync(string id, string reference, CancellationToken cancellationToken)
    {
        return Task.FromResult<DynamicsSubmission?>(_cache[id]);
    }
}