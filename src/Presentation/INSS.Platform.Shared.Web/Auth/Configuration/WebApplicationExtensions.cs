using Microsoft.AspNetCore.Builder;

namespace INSS.Platform.Shared.Web.Auth.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication UseAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
