using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Infrastructure;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.Canonical.API;

/// <summary>
/// Provides extension methods for configuring persistence-related services for User Management.
/// </summary>
public static class PersistenceExtensions
{
    /// <summary>
    /// Registers the <see cref="CanonicalDbContext"/> with the dependency injection container,
    /// using the SQL Server connection string from configuration.
    /// </summary>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="configuration">The application configuration containing the connection string.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the SQL Server connection string is not configured.
    /// </exception>
    public static IServiceCollection AddCanonicalDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CanonicalDbContext>(options =>
        {
            string? sqlConnectionString = configuration.GetConnectionString("CanonicalDb");

            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                throw new InvalidOperationException("SQL Server connection string is not configured.  Update appsettings.json or set the environment variable (ConnectionStrings__CanonicalDb)");
            }

            options.UseSqlServer(sqlConnectionString);
        });

        return services;
    }

    /// <summary>
    /// Registers repository interfaces and their implementations with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the repositories to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IBankDetailsRepository, BankDetailsRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();

        return services;
    }
}
