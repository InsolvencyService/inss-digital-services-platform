using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global - Binding

namespace Inss.FormsSubmission.Service.Options;

public sealed class TokenOptions
{
    [Required]
    public string ClientId { get; init; }
    
    [Required]
    public string Issuer { get; init; }
    
    [Required]
    public string JwtPrivateKey { get; init; }
}

