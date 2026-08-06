using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Infrastructure.Providers;

public sealed class NoSessionAppProvider : IAppProvider
{
    private readonly IAppFactory _appFactory;
    private readonly IUserSessionProvider _userSessionProvider;
    
    public NoSessionAppProvider(IAppFactory appFactory, IUserSessionProvider userSessionProvider)
    {
        _appFactory = appFactory;
        _userSessionProvider = userSessionProvider;
    }
    
    public async ValueTask<AppModel> GetAsync()
    {
        (SessionId SessionId, string Email) session = await _userSessionProvider.ResolveAsync();
        AppModel app = await _appFactory.CreateAsync(session.SessionId);
        app.Email = session.Email;
        return app;
    }

    public ValueTask SaveAsync(AppModel app)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(AppModel app)
    {
        return ValueTask.CompletedTask;
    }
}