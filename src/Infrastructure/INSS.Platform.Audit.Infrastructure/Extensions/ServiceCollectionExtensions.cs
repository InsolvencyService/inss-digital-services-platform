using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Audit.Application.Options;
using INSS.Platform.Audit.Application.Users.Handlers;
using INSS.Platform.Audit.Domain;
using INSS.Platform.Events.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Audit.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuditInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IAuditService, EventGridAuditService>();
        services.AddScoped<IDomainEventHandler<UserDetailsAddedEvent>, UserDetailsAddedHandler>();
        services.AddScoped<IDomainEventHandler<UserIncomeAddedEvent>, UserIncomeAddedHandler>();
        services.AddScoped<IDomainEventHandler<UserBankDetailsAddedEvent>, UserBankDetailsAddedHandler>();

        services.AddOptions<AuditOptions>()
            .Bind(configuration.GetSection("Audit"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}