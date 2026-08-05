using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Frontend.AspNetCore;
using Inss.Platform.Component.Binding;
using Inss.Platform.Component.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddComponents(IConfiguration configuration)
        {
            services.AddOptions<HeaderOptions>()
                .Bind(configuration.GetSection("Header"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<FooterOptions>()
                .Bind(configuration.GetSection("Footer"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<AnalyticsOptions>()
                .Bind(configuration.GetSection("Analytics"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddControllersWithViews(o => o.ModelBinderProviders.Insert(0, new PageComponentBinderProvider()));
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Add("/Views/Components/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Components/Parts/{0}.cshtml");
            });
            services.AddHttpClient();
            services.AddGovUkFrontend();
            services.AddHealthChecks();
            services.AddOpenTelemetry().UseAzureMonitor();
            return services;
        }
    }
}