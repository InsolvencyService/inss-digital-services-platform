using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Infrastructure.Clients;
using INSS.Platform.Portal.Infrastructure.Extensions;
using INSS.Platform.Portal.Web.Components.Register;
using INSS.Platform.Shared.Web.Auth.Configuration;
using INSS.Platform.Shared.Web.Cache.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGovUkFrontend(options => options.Rebrand = true);
builder.Services.AddInfrastructure();

IMvcBuilder mvcBuilder = builder.Services.AddControllersWithViews()
    // Register the views in the /Views/Shared folder in the shared components assembly.
    .AddApplicationPart(typeof(ClientRegistration).Assembly) 
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });

builder.Services.AddCacheConfiguration(builder.Environment);

builder.Services.AddAuthenticationConfiguration(builder.Configuration, mvcBuilder, builder.Environment);

builder.Services.AddHttpClient();

builder.Services.AddScoped<IFormApiClient, FormApiClient>();

builder.Services.AddScoped<IEventTrackerClient, RybbitEventTrackerClient>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCacheConfiguration();

app.UseAuthenticationConfiguration();

app.MapStaticAssets();

app.UseGovUkFrontend();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
