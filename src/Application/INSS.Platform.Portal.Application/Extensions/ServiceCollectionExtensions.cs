using INSS.Platform.Portal.Application.Options;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Application.Validation;
using INSS.Platform.Portal.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services, 
        Action<ApplicationOptions>? configure = null)
    {
        ApplicationOptions options = new();
        configure?.Invoke(options);

        services.AddSingleton<IModelStateValidator<BankAccountModel>, BankAccountModelStateValidator>();
        services.AddSingleton<IModelTypeService>(_ => new ModelTypeService(options.ModelAssemblies.ToArray()));
        services.AddTransient<IFormService, FormService>();
        services.AddSingleton<IFormSerializationService, FormSerializationService>();
        return services;
    }
}