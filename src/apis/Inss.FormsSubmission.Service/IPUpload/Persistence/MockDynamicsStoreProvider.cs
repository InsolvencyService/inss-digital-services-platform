using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable CollectionNeverQueried.Local

namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

[ExcludeFromCodeCoverage]
public sealed class MockDynamicsStoreProvider : IDynamicsStoreProvider
{
    private static readonly ConcurrentDictionary<string, DynamicsSubmission> _cache = new();
    
    public Task StoreAsync(DynamicsSubmission submission)
    {
        _cache[submission.Id] = submission;
        return Task.CompletedTask;
    }
}