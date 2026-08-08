using Inss.Platform.Application.Factories;
using Inss.Platform.Component;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Ipus;

public sealed class IpusAppBuilder : AppBuilder
{
    public override PagePath[] Build(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAppFactory>(_ => new AppFactory([]));//[searchTermPage, searchResultPage, searchResultDetailPage]));

        return []; //searchTermPage.Path, searchResultPage.Path, searchResultDetailPage.Path];
    }
}