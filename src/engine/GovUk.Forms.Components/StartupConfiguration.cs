using System.Reflection;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Controllers;
using GovUk.Forms.Components.Handlers;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Serialization;
using GovUk.Forms.Infrastructure.Extensions;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(GovUk.Forms.Components.StartupConfiguration))]

namespace GovUk.Forms.Components;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            List<Assembly> modelAssemblies = [typeof(PageModel).Assembly];
                
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                         a.FullName?.StartsWith("Demo.", StringComparison.OrdinalIgnoreCase) == true ||
                         a.FullName?.StartsWith("Inss.", StringComparison.OrdinalIgnoreCase) == true))
            {
                modelAssemblies.Add(assembly);
            }
            
            FormSerializer.Initialize(modelAssemblies.ToArray());

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddApplication();
            services.AddInfrastructure();
            services
                .AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new ContentModelBinderProvider()))
                .AddApplicationPart(typeof(FormController).Assembly);
            services.AddSingleton<ITypeNameResolver>(_ => new TypeNameResolver(modelAssemblies.ToArray()));
            services.AddHttpClient();
            services.AddGovUkFrontend(o => o.Rebrand = true);
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
            
            app.UseGovUkFrontend();
            
            app.UseExceptionHandler("/error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            
            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "error",
                        pattern: "/error",
                        defaults: new { controller = "Error", action = "Index" })
                    .WithStaticAssets();

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
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}")
                    .WithStaticAssets();
                
                endpoints.MapStaticAssets();
            });
        });
    }
}