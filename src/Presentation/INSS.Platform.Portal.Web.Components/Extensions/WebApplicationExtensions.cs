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
            defaults: new { controller = "Form", action = "Index" });
        
        foreach (SectionModel section in form.Sections)
        {
            app.MapControllerRoute(name: $"{section.PathName}",
                pattern: section.PageUrl + "/summary",
                defaults: new { controller = "Form", action = "Index" });
            
            foreach (BaseModel page in section.Pages)
            {
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}",
                    pattern: page.PageUrl,
                    defaults: new { controller = "Form", action = "Index" });
                
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-change",
                    pattern: page.PageUrl + "/change",
                    defaults: new { controller = "Form", action = "Change" });
                
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-remove",
                    pattern: page.PageUrl + "/remove",
                    defaults: new { controller = "Form", action = "Remove" });
                
                app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-add",
                    pattern: page.PageUrl + "/add",
                    defaults: new { controller = "Form", action = "Add" });

                if (page is AddAnotherModel addAnother)
                {
                    foreach (BaseModel subPage in addAnother.Items[0]) // TODO: Assumption
                    {
                        app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}",
                            pattern: subPage.PageUrl,
                            defaults: new { controller = "Form", action = "Index" });
                
                        // app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-change",
                        //     pattern: subPage.PageUrl + "/change",
                        //     defaults: new { controller = "Form", action = "Change" });
                        //
                        // app.MapControllerRoute(name: $"{section.PathName}-{page.PathName}-remove",
                        //     pattern: subPage.PageUrl + "/remove",
                        //     defaults: new { controller = "Form", action = "Remove" });
                    }
                }
            }
        }
   
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        return app;
    }
}