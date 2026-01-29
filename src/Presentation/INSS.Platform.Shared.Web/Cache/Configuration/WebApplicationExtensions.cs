using Microsoft.AspNetCore.Builder;

namespace INSS.Platform.Shared.Web.Cache.Configuration;

public static class WebApplicationExtensions
{
    public static WebApplication UseCacheConfiguration(this WebApplication app)
    {
        app.UseSession();

        return app;
    }
}
