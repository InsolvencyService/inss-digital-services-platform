using Inss.Common.Infrastructure.Options;

namespace Inss.FormsSubmission.Service.Options;

public class DynamicsOptions : ExternalApiOptions
{
    public string ClientId { get; init; }
    
    public string ClientSecret { get; init; }
    
    public string TenantId { get; init; }
}