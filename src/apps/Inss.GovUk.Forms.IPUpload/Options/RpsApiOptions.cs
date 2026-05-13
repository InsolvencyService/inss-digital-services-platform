using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Components.Options;

namespace Inss.GovUk.Forms.IPUpload.Options;

public sealed class RpsApiOptions : ExternalApiOptions
{
    [Required]
    public string ConnectionName { get; init; }
    
    [Required]
    public string KeyName { get; init; }
    
    [Required]
    public string Key { get; init; }
}