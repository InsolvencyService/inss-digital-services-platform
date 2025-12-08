using System.Collections.Concurrent;
using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Domain.Exceptions;

namespace INSS.Platform.Portal.Infrastructure;

public sealed class TestFormStateService : IFormStateService
{
    private readonly IUserSessionResolver _userSessionResolver;
    private static readonly ConcurrentDictionary<string, FormModel> _cache = new();

    public TestFormStateService(IUserSessionResolver userSessionResolver)
    {
        _userSessionResolver = userSessionResolver;
    }
    
    public Task<bool> FormExistsAsync()
    {
        string sessionId = _userSessionResolver.GetUserId();
        return Task.FromResult(_cache.ContainsKey(sessionId));
    }

    public Task<FormModel> GetAsync()
    {
        string sessionId = _userSessionResolver.GetUserId();
        
        if (_cache.TryGetValue(sessionId, out FormModel? model))
        {
            return Task.FromResult(model);
        }

        throw new FormModelException($"Unable to find the form model for the session {sessionId}");
    }
    
    public Task SaveAsync(FormModel model)
    {
        string sessionId = _userSessionResolver.GetUserId();
        _cache[sessionId] = model;
        return Task.CompletedTask;
    }
}