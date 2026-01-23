using GovUk.Frontend.AspNetCore;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Infrastructure.Extensions;
using INSS.Platform.Portal.Web.Components.Register;
using INSS.Platform.Shared.Web.Auth.Configuration;

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

builder.Services.AddSession();

builder.Services.AddAuthenticationConfiguration(builder.Configuration, mvcBuilder, builder.Environment);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.Services.AddScoped<IFormApiClient, FormApiClient>();

builder.Services.AddScoped<IFormCacheClient, FormCacheClient>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseGovUkFrontend();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
