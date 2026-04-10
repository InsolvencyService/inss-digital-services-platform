using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddSingleton<IFormStorageProvider, TestFormStorageProvider>();
            services.AddSingleton<IStaticContentProvider, TestStaticContentProvider>();
            services.AddSingleton<IFormProvider, TestFormProvider>();
            return services;
        }
    }
}