using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Infrastructure.Clients;
using INSS.Platform.Shared.Web.Auth.Controllers;
using INSS.Platform.Shared.Web.Auth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace INSS.Platform.Shared.Web.Auth.Configuration;

/// <summary>
/// Provides extension methods to configure authentication services for INSS Platform web applications.
/// </summary>
/// <remarks>
/// This configuration sets up cookie-based authentication using the default cookie scheme,
/// applies secure cookie policies, and binds the <see cref="AuthOptions"/> from configuration
/// with data annotation validation on startup. Additionally, it registers <see cref="IJwtAuthenticationService"/>
/// for JWT-related operations used by the authentication flow.
/// </remarks>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds and configures cookie-based authentication and registers the MVC authentication controller <see cref="AccountController"/>,
    /// binds <see cref="AuthOptions"/> from configuration with validation, and registers <see cref="IJwtAuthenticationService"/>.
    /// </summary>
    /// <param name="services">The service collection to which authentication and related services are added.</param>
    /// <param name="configuration">The application configuration used to bind <see cref="AuthOptions"/> from the <c>Authentication</c> section.</param>
    /// <param name="mvcBuilder">The MVC builder used to include authentication-related controllers via <see cref="ApplicationPartManager"/>.</param>
    /// <param name="environment">The host environment providing application and environment names for cookie configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further configuration chaining.</returns>
    /// <remarks>
    /// This method:
    /// - Configures <see cref="CookieAuthenticationDefaults.AuthenticationScheme"/> with secure cookie settings.
    /// - Sets login and logout paths to <c>/Account/Login</c> and <c>/Account/Logout</c>.
    /// - Ensures cookies are <see cref="CookieSecurePolicy.Always"/> and <see cref="SameSiteMode.Lax"/>.
    /// - Registers the authentication controllers by adding the assembly containing <see cref="AccountController"/>.
    /// - Binds and validates <see cref="AuthOptions"/> on startup.
    /// - Registers <see cref="JwtAuthenticationService"/> as the scoped implementation of <see cref="IJwtAuthenticationService"/>.
    /// </remarks>
    public static IServiceCollection AddAuthenticationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IMvcBuilder mvcBuilder,
        IHostEnvironment environment)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = CreateApplicationCookieName(environment);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

        mvcBuilder.AddApplicationPart(typeof(AccountController).Assembly);

        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
        services.AddScoped<IEventTrackerClient, RybbitEventTrackerClient>();

        return services;
    }

    /// <summary>
    /// Creates a safe, deterministic cookie name based on the application and environment names.
    /// </summary>
    /// <param name="environment">The host environment providing <see cref="IHostEnvironment.ApplicationName"/> and <see cref="IHostEnvironment.EnvironmentName"/>.</param>
    /// <returns>
    /// A cookie name in the form <c>{ApplicationName}.{EnvironmentName}.Auth</c>,
    /// where non-alphanumeric characters (except <c>.</c>) are removed. Fallbacks to
    /// <c>UnknownApplication</c> and <c>UnknownEnvironment</c> if values are missing.
    /// </returns>
    /// <remarks>
    /// This method sanitizes the application and environment names to include only letters, digits, and periods,
    /// ensuring the resulting cookie name is valid and consistent across deployments.
    /// </remarks>
    private static string CreateApplicationCookieName(IHostEnvironment environment)
    {
        string appName = string.IsNullOrWhiteSpace(environment.ApplicationName) ? "UnknownApplication" : environment.ApplicationName;
        string envName = string.IsNullOrWhiteSpace(environment.EnvironmentName) ? "UnknownEnvironment" : environment.EnvironmentName;

        string safeApp = new([.. appName.Where(ch => char.IsLetterOrDigit(ch) || ch == '.')]);
        string safeEnv = new([.. envName.Where(ch => char.IsLetterOrDigit(ch) || ch == '.')]);

        return $"{safeApp}.{safeEnv}.Auth";
    }
}