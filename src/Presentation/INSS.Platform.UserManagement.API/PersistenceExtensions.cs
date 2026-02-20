using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Infrastructure.Repositories;
using INSS.Platform.UserManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using INSS.Platform.UserManagement.Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.UserManagement.API;

/// <summary>
/// Provides extension methods for configuring persistence-related services for User Management.
/// </summary>
[ExcludeFromCodeCoverage]
public static class PersistenceExtensions
{
    /// <summary>
    /// Registers the <see cref="UserManagementDbContext"/> with the dependency injection container,
    /// using the SQL Server connection string from configuration.
    /// </summary>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="configuration">The application configuration containing the connection string.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the SQL Server connection string is not configured.
    /// </exception>
    public static IServiceCollection AddUserManagementDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserManagementDbContext>(options =>
        {
            string? sqlConnectionString = configuration.GetConnectionString("UserManagementDb");

            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                throw new InvalidOperationException("SQL Server connection string is not configured.  Update appsettings.json or set the environment variable (ConnectionStrings__UserManagementDb)");
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
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAddressTypeRepository, AddressTypeRepository>();
        services.AddScoped<IAuthenticationPolicyProviderRepository, AuthenticationPolicyProviderRepository>();
        services.AddScoped<IAuthenticationPolicyRepository, AuthenticationPolicyRepository>();
        services.AddScoped<IAuthenticationProviderMetadataRepository, AuthenticationProviderMetadataRepository>();
        services.AddScoped<IAuthenticationProviderRepository, AuthenticationProviderRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IIndividualRepository, IndividualRepository>();
        services.AddScoped<IOrganisationRepository, OrganisationRepository>();
        services.AddScoped<IPartyAuthenticationProviderMetadataRepository, PartyAuthenticationProviderMetadataRepository>();
        services.AddScoped<IPartyProductRoleRepository, PartyProductRoleRepository>();
        services.AddScoped<IPartyRelationshipRepository, PartyRelationshipRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IPartyTypeRepository, PartyTypeRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IProductAuthenticationPolicyProviderRepository, ProductAuthenticationPolicyProviderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductRoleRepository, ProductRoleRepository>();
        services.AddScoped<IProductRoleResourcePermissionRepository, ProductRoleResourcePermissionRepository>();
        services.AddScoped<IRelationshipTypeRepository, RelationshipTypeRepository>();
        services.AddScoped<IResourcePermissionRepository, ResourcePermissionRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IRoleMetadataRepository, RoleMetadataRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }

    /// <summary>
    /// Registers repository-related services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRelationshipTypeLookupService, RelationshipTypeLookupService>();
        
        return services;
    }
}
