using Inss.Auth.RpsProvider.Options;

namespace Inss.Auth.RpsProvider.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddIdentityProviderOptions()
        {
            builder.Services.AddOptions<ProviderOptions>()
                .Bind(builder.Configuration.GetSection("Provider"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            return builder;
        }
    }
}