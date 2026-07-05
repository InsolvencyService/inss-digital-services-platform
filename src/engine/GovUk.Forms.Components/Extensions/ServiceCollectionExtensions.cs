using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddFormEngine(IConfiguration configuration)
        {
            services.AddSingleton<IContentBinderFactory, ContentBinderFactory>();
            services.AddSingleton<IContentBinder, DefaultContentBinder>();
            services.AddKeyedSingleton<IContentBinder, FileContentBinder>(typeof(FileUploadModel).FullName);
            services.AddSingleton<ITypeNameResolver, TypeNameResolver>();
            services.AddSingleton<IPageMetaDataResolver, PageMetaDataResolver>();
            services.AddApplication();
            services.AddInfrastructure(configuration);
            return services;
        }
    }
}