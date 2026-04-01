using Inss.Auth.Broker.Options;

namespace Inss.Auth.Broker.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder AddIdentityProviderOptions()
        {
            builder.Services.AddOptions<RpsIdentityProviderOptions>()
                .Bind(builder.Configuration.GetSection("IdentityProviders:Rps"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            builder.Services.AddOptions<OneLoginIdentityProviderOptions>()
                .Bind(builder.Configuration.GetSection("IdentityProviders:OneLogin"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            builder.Services.AddOptions<EntraIdentityProviderOptions>()
                .Bind(builder.Configuration.GetSection("IdentityProviders:Entra"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            return builder;
        }
        
        public WebApplicationBuilder AddBrokerOptions()
        {
            builder.Services.AddOptions<BrokerOptions>()
                .Bind(builder.Configuration.GetSection("Broker"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            return builder;
        }
    }
}