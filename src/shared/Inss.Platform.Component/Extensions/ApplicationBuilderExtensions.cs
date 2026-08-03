using GovUk.Frontend.AspNetCore;
using Inss.Platform.Application.Factories;
using Inss.Platform.Component.Options;
using Inss.Platform.Domain.Primitives;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Inss.Platform.Component.Extensions;

public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseComponents()
        {
            app.Use(async (context, next) =>
            {
                IOptions<AnalyticsOptions> analyticsOptions = context.RequestServices.GetRequiredService<IOptions<AnalyticsOptions>>();
                context.Response.Headers.XFrameOptions = "DENY";
                context.Response.Headers.ContentSecurityPolicy = $"default-src 'self' https://app.rybbit.io {analyticsOptions.Value.SecurityHash}";
                context.Response.Headers.XContentTypeOptions = "nosniff";
                context.Response.Headers.XXSSProtection = "1; mode=block";
                await next();
            });
            
            app.UseExceptionHandler("/error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseGovUkFrontend();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");
            app.UseStaticFiles();
            return app;
        }
        
        public IApplicationBuilder UsePageEngine(PagePath[] pagePaths)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                foreach (PagePath pagePath in pagePaths)
                {
                    endpoints.MapControllerRoute(
                            name: pagePath.Value,
                            pattern: pagePath.Value,
                            defaults: new { controller = "Page", action = "Edit" })
                        .WithStaticAssets();    
                }
                
                /*
                IServiceProvider serviceProvider = endpoints.ServiceProvider;
                IFormFactory formProvider = serviceProvider.GetRequiredService<IFormFactory>();
                FormModel form = formProvider.Create();

                endpoints.MapControllerRoute(
                        name: $"{form.Path.Value}/edit",
                        pattern: form.Path.Value,
                        defaults: new { controller = "Form", action = "Edit" })
                    .WithStaticAssets();

                foreach (PageModel page in form.GetAllPages())
                {
                    endpoints.MapControllerRoute(
                            name: $"{page.Path.Value}/edit",
                            pattern: page.Path.Value,
                            defaults: new { controller = "Form", action = "Edit" })
                        .WithStaticAssets();
                }

                endpoints.MapControllerRoute(
                        name: "FormSignOut",
                        pattern: "sign-out",
                        defaults: new { controller = "Form", action = "LogOut" })
                    .WithStaticAssets();
                    */
            });

            return app;
        }
    }
}