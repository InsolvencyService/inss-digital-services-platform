using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Providers;
using GovUk.Frontend.AspNetCore;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Application.Services;
using Inss.Auth.RpsProvider.Infrastructure.Clients;
using Inss.Auth.RpsProvider.Infrastructure.Providers;
using Inss.Auth.RpsProvider.Options;
using Inss.Common.Infrastructure;
using Inss.Common.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

[assembly: HostingStartup(typeof(Inss.Auth.RpsProvider.StartupConfiguration))]

namespace Inss.Auth.RpsProvider;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            IConfigurationRoot config = configurationBuilder.Build();
            string? configFileOverride = config["config"];
            
            if (configFileOverride is not null && File.Exists(configFileOverride)){
                configurationBuilder.AddJsonFile(configFileOverride, optional: true);
            }
        });
        
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            
            services.AddOptions<ProviderOptions>()
                .Bind(context.Configuration.GetSection("Provider"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<LoginOptions>()
                .Bind(context.Configuration.GetSection("Login"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<ILoginService, LoginService>();

            ExternalApiOptions loginOptions = context.Configuration.GetSection("RpsLogin").Get<ExternalApiOptions>()!;

            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IUserAuthenticationPageClient, MockUserAuthenticationPageClient>();
                services.AddSingleton<IUserAuthenticationClient, MockUserAuthenticationClient>();
            }
            else
            {
                CookieContainer cookieContainer = new();
                services.AddScoped<CookieContainer>(_ => cookieContainer);
                
                services.AddHttpClient<IUserAuthenticationPageClient, UserAuthenticationPageClient>(client =>
                    {
                        client.BaseAddress = new Uri(loginOptions.Url);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = cookieContainer, UseCookies = true, AllowAutoRedirect = true
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(loginOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, loginOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp, 
                        loginOptions.CountBeforeBreaking, loginOptions.BreakDurationSeconds));

                services.AddHttpClient<IUserAuthenticationClient, UserAuthenticationClient>(client =>
                    {
                        client.BaseAddress = new Uri(loginOptions.Url);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = cookieContainer, UseCookies = true, AllowAutoRedirect = false
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(loginOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, loginOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp,
                        loginOptions.CountBeforeBreaking, loginOptions.BreakDurationSeconds));
            }

            services.AddSingleton<IUserAuthStoreProvider, TestUserAuthStoreProvider>();
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            services.AddScoped<IPagePropertiesProvider, PagePropertiesProvider>();
            services.AddGovUkFrontend();
            services.AddControllersWithViews();
            services.AddOpenTelemetry().UseAzureMonitor();
        });
        
        builder.Configure(app =>
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedHost |
                    ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();
            app.UseGovUkFrontend();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        });
    }
}