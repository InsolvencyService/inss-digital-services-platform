using GovUk.Frontend.AspNetCore;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Extensions;
using Inss.Auth.Broker.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.AddBrokerOptions();
builder.AddIdentityProviderOptions();
builder.Services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
    .AddCookie()
    .AddOneLogin()
    .AddRps()
    .AddEntra();
builder.Services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
builder.Services.AddSingleton<IAuthCodeStoreProvider, TestAuthCodeStoreProvider>();
builder.Services.AddGovUkFrontend();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseGovUkFrontend();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();