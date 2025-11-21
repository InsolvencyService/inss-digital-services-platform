using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IJourneyService, JourneyService>();
        services.AddTransient<IModelService<BankAccountModel>, BankAccountService>();
        services.AddTransient<IModelService<AddressModel>, AddressService>();
        services.AddTransient<IModelService<FullNameModel>, FullNameService>();
        services.AddTransient<IModelService<SectionModel>, SectionService>();
        services.AddTransient<IModelService<FormModel>, FormService>();
        services.AddTransient<IModelService<SummaryListModel>, SummaryListService>();
        return services;
    }
}