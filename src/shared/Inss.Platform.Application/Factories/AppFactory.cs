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

    public ValueTask<AppModel> CreateAsync(SessionId session)
    {
        AppModel app = new() { Session = session, Pages = [.._pages] };
        string json = AppSerialization.Serialize(app);
        return ValueTask.FromResult(AppSerialization.Deserialize(json));
    }
}