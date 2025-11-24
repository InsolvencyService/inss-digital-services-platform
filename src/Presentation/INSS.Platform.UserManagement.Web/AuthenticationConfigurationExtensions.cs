using INSS.Platform.UserManagement.Web.Models;
using INSS.Platform.UserManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace INSS.Platform.UserManagement.Web;

public static class AuthenticationConfigurationExtensions
{
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "UserManagement.Auth";
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
        return services;
    }
}
