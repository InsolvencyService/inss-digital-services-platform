using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace INSS.Platform.Portal.Web.Components.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseComponents(this WebApplication app)
    {
        app.UseGovUkFrontend();

        IFormModelFactory modelDataFactory = app.Services.GetRequiredService<IFormModelFactory>();

        FormModel form = modelDataFactory.CreateAsync().Result;

        app.MapControllerRoute(name: form.PathName,
            pattern: $"{form.PathName}",
            defaults: new { controller = form.Controller, action = form.Action });

        app.MapControllerRoute(name: "confirm",
            pattern: "tasks/about-you/summary-list/confirm",
            defaults: new { controller = "Confirm", action = "Index" });

        foreach (SectionModel section in form.Sections)
        {
            app.MapControllerRoute(name: "summary",
                pattern: section.PageUrl,
                defaults: new { controller = "Summary", action = "Index" });
            
            foreach (PageModel page in section.Pages)
            {
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}",
                    pattern: page.PageUrl,
                    defaults: new { controller = page.Controller, action = page.Action });

                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-change",
                    pattern: page.PageUrl + "/change",
                    defaults: new { controller = page.Controller, action = "Change" });

                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-remove",
                    pattern: page.PageUrl + "/remove",
                    defaults: new { controller = page.Controller, action = "Remove" });

                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-post-remove",
                    pattern: page.PageUrl + "/post-remove",
                    defaults: new { controller = page.Controller, action = "PostRemove" });
            }
        }

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        return app;
    }
}