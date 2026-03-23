using GovUk.Forms.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddSingleton<IFormService, FormService>();
            services.AddSingleton<ISubmitFormService, SubmitFormService>();
            return services;
        }
    }
}