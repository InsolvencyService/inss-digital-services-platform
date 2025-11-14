using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Web.Components.Extensions;
using INSS.Platform.Portal.Web.Factories;
using INSS.Platform.Portal.Web.Models;
using INSS.Platform.Portal.Web.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddComponents();

builder.Services.AddTransient<IFormModelFactory, WebAppFormModelFactory>();

builder.Services.AddTransient<IModelService<HomeValueModel>, HomeValueService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseComponents();

app.Run();