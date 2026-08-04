using GovUk.Forms.Application.Factories;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Domain;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GovUk.Forms.Components.Extensions;

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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Start}/{action=Index}/{id?}")
                    .WithStaticAssets();

                endpoints.MapStaticAssets();
            });
            return app;
        }
        
        public IApplicationBuilder UseFormEngine()
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

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
            });

            return app;
        }
    }
}