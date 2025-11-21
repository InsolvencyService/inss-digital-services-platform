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
            }
        }

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        return app;
    }
}