using System.Collections.Concurrent;
using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Infrastructure;

public sealed class TestFormStateService : IFormStateService
{
    private readonly IUserSessionResolver _userSessionResolver;
    private static readonly ConcurrentDictionary<string, FormModel> _cache2 = new();

    public TestFormStateService(IUserSessionResolver userSessionResolver)
    {
        _userSessionResolver = userSessionResolver;
    }
    
    public Task<bool> FormExistsAsync()
    {
        string sessionId = _userSessionResolver.GetUserId();
        return Task.FromResult(_cache2.ContainsKey(sessionId));
    }

    public Task<FormModel> GetAsync()
    {
        string sessionId = _userSessionResolver.GetUserId();
        
        if (_cache2.TryGetValue(sessionId, out var model))
        {
            return Task.FromResult(model);
        }

        throw new InvalidOperationException($"Unable to find the form model for the session {sessionId}");
    }
    
    public Task SaveAsync(FormModel model)
    {
        string sessionId = _userSessionResolver.GetUserId();
        _cache2[sessionId] = model;
        return Task.CompletedTask;
    }
}