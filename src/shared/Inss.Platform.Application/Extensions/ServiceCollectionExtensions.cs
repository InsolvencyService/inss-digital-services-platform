using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INextPageNavigator, DefaultNextPageNavigator>();
            return services;
        }
    }
}