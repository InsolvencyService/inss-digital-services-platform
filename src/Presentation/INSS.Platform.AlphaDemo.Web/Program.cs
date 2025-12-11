using INSS.Platform.AlphaDemo.Web.Factories;
using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Web.Components.Extensions;
using INSS.Platform.Shared.Web.Auth.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddComponents();

builder.Services.AddTransient<IFormModelFactory, WebAppFormModelFactory>();

IMvcBuilder mvcBuilder = builder.Services.AddControllersWithViews();

builder.Services.AddAuthenticationConfiguration(builder.Configuration, mvcBuilder, builder.Environment);

WebApplication app = builder.Build();

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

app.UseComponents();

app.Run();
