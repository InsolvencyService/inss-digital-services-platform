using Inss.Platform.Application.Providers;
using Inss.Platform.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddSingleton<IAppProvider, TempAppProvider>();
            return services;
        }
    }
}