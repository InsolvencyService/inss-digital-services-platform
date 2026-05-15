using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Controllers;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Extensions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(GovUk.Forms.Components.StartupConfiguration))]

namespace GovUk.Forms.Components;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
        });
        
        builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<HeaderOptions>()
                .Bind(context.Configuration.GetSection("Header"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<FooterOptions>()
                .Bind(context.Configuration.GetSection("Footer"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<AnalyticsOptions>()
                .Bind(context.Configuration.GetSection("Analytics"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            ComponentOptions componentOptions = new();
            context.Configuration.GetSection("Components").Bind(componentOptions);

            if (componentOptions.BootstrapFormFramework)
            {
                services.AddApplication();
                services.AddInfrastructure(context.Configuration);
            }

            IMvcBuilder mvcBuilder = services
                .AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new ContentModelBinderProvider()))
                .AddApplicationPart(typeof(FormController).Assembly);
            RemoveNonHostedDiscoveredParts(mvcBuilder);
            
            services.AddSingleton<ITypeNameResolver, TypeNameResolver>();
            services.AddHttpClient();
            services.AddGovUkFrontend();
            services.AddHealthChecks();
        });
        
        builder.Configure(app =>
        {
            /*
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            */
            
            app.Use(async (context, next) =>
            {
                context.Response.Headers.XFrameOptions = "DENY";
                context.Response.Headers.ContentSecurityPolicy = 
                    "default-src 'self' https://app.rybbit.io 'sha256-GUQ5ad8JK5KmEWmROf3LZd9ge94daqNvd8xy9YS1iDw=' " +
                    "'sha256-+MPr4O+XRBNAduB7gNJMvYtSAF5bNPiBYOUmvIx/CSA='";
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

                IServiceProvider serviceProvider = endpoints.ServiceProvider;
                
                IEnumerable<IWebRoot> webRoots = serviceProvider.GetServices<IWebRoot>();
            
                foreach (IWebRoot webRoot in webRoots)
                {
                    IFormProvider formProvider = serviceProvider.GetRequiredService<IFormProvider>();
                    FormModel form = formProvider.Create(webRoot.Root);

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
                }
            
                endpoints.MapControllerRoute(
                        name: "FormSignOut",
                        pattern: "sign-out",
                        defaults: new { controller = "Form", action = "LogOut" })
                    .WithStaticAssets();
                
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Start}/{action=Index}/{id?}")
                    .WithStaticAssets();
                
                endpoints.MapStaticAssets();
            });
        });
    }

    private static string[] GetHostedAssemblyNames()
    {
        string environmentName = Environment.GetEnvironmentVariable("DOTNET_HOSTINGSTARTUPASSEMBLIES")!;
        var hostedAssemblies = environmentName
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        hostedAssemblies.Add("GovUk.Forms.HostApp");
        hostedAssemblies.Add("GovUk.Frontend.AspNetCore");
        return hostedAssemblies.ToArray();
    }

    private static void RemoveNonHostedDiscoveredParts(IMvcBuilder mvcBuilder)
    {
        string[] hostedAssemblyNames = GetHostedAssemblyNames();
        ApplicationPartManager partManager = mvcBuilder.PartManager;
        ApplicationPart[] applicationPartsToRemove = partManager.ApplicationParts.Where(
            part => !hostedAssemblyNames.Contains(part.Name)).ToArray();
            
        foreach (var part in applicationPartsToRemove)
        {
            partManager.ApplicationParts.Remove(part);
        }
    }
}