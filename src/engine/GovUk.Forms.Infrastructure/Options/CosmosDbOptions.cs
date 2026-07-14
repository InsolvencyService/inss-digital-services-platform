// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace GovUk.Forms.Infrastructure.Options;

public sealed class CosmosDbOptions
{
    public string? AccountEndpoint { get; init; }
    
    public string? ConnectionString { get; init; }
    
    public string DatabaseName { get; init; }
    
    public string ContainerName { get; init; }
}