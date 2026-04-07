using Azure.Monitor.OpenTelemetry.AspNetCore;

namespace GovUk.Forms.HostApp.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddHostServices()
        {
            builder.Services.AddOpenTelemetry().UseAzureMonitor();
            return builder;
        }
    }
}