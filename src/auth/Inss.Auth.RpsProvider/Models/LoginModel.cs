using System.ComponentModel.DataAnnotations;

namespace Inss.Auth.RpsProvider.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Enter an email address")]
    [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
    public string Email { get; init; }
    
    [Required(ErrorMessage = "Enter a password")]
    public string Password { get; init; }    
    
    public string ReturnUrl { get; init; }
    
    public string ForgotPasswordUrl { get; init; }
}