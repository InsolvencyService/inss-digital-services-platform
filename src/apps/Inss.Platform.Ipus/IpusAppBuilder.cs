using Inss.Platform.Application.Factories;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Component;
using Inss.Platform.Component.Builders;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Ipus;

public sealed class IpusAppBuilder : AppBuilder
{
    public override PagePath[] Build(IServiceCollection services)
    {
        services.AddSingleton<IAppFactory>(_ => new AppFactory([]));//[searchTermPage, searchResultPage, searchResultDetailPage]));

        return []; //searchTermPage.Path, searchResultPage.Path, searchResultDetailPage.Path];
    }
}