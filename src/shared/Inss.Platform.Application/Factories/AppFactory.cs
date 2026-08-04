using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Serialization;

namespace Inss.Platform.Application.Factories;

public sealed class AppFactory : IAppFactory
{
    private readonly PageModel[] _pages;

    public AppFactory(PageModel[] pages)
    {
        _pages = pages;
    }

    public ValueTask<App> CreateAsync(SessionId session)
    {
        App appPages = new() { Session = session, Pages = [.._pages] };
        string json = AppSerialization.Serialize(appPages);
        return ValueTask.FromResult(AppSerialization.Deserialize(json));
    }
}