using GovUk.Forms.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddTransient<IFormService, FormService>();
            services.AddTransient<IUserFormService, UserFormService>();
            services.AddTransient<ISubmitFormService, SubmitFormService>();
            return services;
        }
    }
}