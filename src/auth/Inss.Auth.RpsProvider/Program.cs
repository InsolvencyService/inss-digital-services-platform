using GovUk.Frontend.AspNetCore;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Extensions;
using Inss.Auth.RpsProvider.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddIdentityProviderOptions();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/login");
builder.Services.AddSingleton<IUserAuthStoreProvider, TestUserAuthStoreProvider>();
builder.Services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
builder.Services.AddGovUkFrontend();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseGovUkFrontend();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();