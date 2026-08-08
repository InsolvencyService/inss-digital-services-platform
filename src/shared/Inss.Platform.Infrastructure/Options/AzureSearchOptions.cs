using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global - options

namespace Inss.Platform.Infrastructure.Options;

public sealed class AzureSearchOptions
{
    [Required]
    public string Endpoint { get; init; }

    [Required]
    public string IndexName { get; init; }

    [Required]
    public string ApiKey { get; init; }

    public string ApiVersion { get; init; }
    
    [Required]
    public string ConfigPath { get; init; }
}