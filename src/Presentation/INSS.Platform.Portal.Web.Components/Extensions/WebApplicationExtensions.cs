using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Web.Components.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseComponents(this WebApplication app)
    {
        app.UseGovUkFrontend();
        
        var modelDataFactory = app.Services.GetRequiredService<IFormModelFactory>();

        var form = modelDataFactory.CreateAsync().Result;

        app.MapControllerRoute(name: form.PathName,
            pattern: $"{form.PathName}",
            defaults: new { controller = form.Controller, action = form.Action });

        foreach (var section in form.Sections)
        {
            app.MapControllerRoute(name: "summary",
                pattern: section.PageUrl,
                defaults: new { controller = "Summary", action = "Index" });
            
            foreach (var page in section.Pages)
            {
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}",
                    pattern: page.PageUrl,
                    defaults: new { controller = page.Controller, action = page.Action });
            }
        }

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        return app;
    }
}