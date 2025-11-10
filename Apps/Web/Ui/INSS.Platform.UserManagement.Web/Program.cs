using GovUk.Frontend.AspNetCore;
using INSS.Platform.UserManagement.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAuthenticationConfiguration(builder.Configuration);

builder.Services.AddGovUkFrontend(options =>
{
    options.Rebrand = true;
});

WebApplication app = builder.Build();

app.UseGovUkFrontend();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
