using INSS.Platform.AlphaDemo.Web.Factories;
using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Web.Components.Extensions;
using INSS.Platform.Shared.Web.Auth;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddComponents();

builder.Services.AddTransient<IFormModelFactory, WebAppFormModelFactory>();

builder.Services.AddControllersWithViews();

builder.Services.AddOptions<AuthOptions>()
    .Bind(builder.Configuration.GetSection("Authentication"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseComponents();

app.Run();
