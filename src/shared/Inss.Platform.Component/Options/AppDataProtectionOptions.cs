using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Platform.Component.Options;

public sealed class AppDataProtectionOptions
{
    [Required]
    public string AppName { get; init; }
    
    [Required]
    public string StorageAccountName { get; init; }
    
    [Required]
    public string StorageAccountBlobName { get; init; }
    
    [Required]
    public string KeyVaultName { get; init; }
    
    [Required]
    public string KeyVaultKeyName { get; init; }
}