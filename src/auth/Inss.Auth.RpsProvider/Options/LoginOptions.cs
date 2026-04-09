using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Auth.RpsProvider.Options;

public sealed class LoginOptions
{
    [Required]
    public string BackUrl { get; init; }
    
    [Required]
    public string ForgotPasswordUrl { get; init; }
}