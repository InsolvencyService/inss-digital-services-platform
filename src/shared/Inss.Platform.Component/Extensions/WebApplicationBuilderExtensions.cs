using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Inss.Platform.Component.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddConfigOverrideIfExists()
        {
            // This allows you to run the app with config file (like user secrets) and still work in production mode locally.
            // Note that users secrets will not work when ASPNETCORE_ENVIRONMENT is Production
            
            string? configFileOverride = builder.Configuration["config"];
            
            if (configFileOverride is not null && File.Exists(configFileOverride)){
                builder.Configuration.AddJsonFile(configFileOverride, optional: true);
            }
            
            return builder;
        }
    }
}