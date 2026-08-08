using Inss.Common.Infrastructure.Options;
// ReSharper disable UnusedAutoPropertyAccessor.Global - options

namespace Inss.Platform.Ipus.Options;

public class DynamicsOptions : ExternalApiOptions
{
    public string ClientId { get; init; }
    
    public string ClientSecret { get; init; }
    
    public string TenantId { get; init; }
}