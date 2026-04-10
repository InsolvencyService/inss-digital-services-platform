using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Domain.Types;

namespace Inss.Auth.RpsProvider.Models;

public class LoginModel
{
    public EmailAddress Email { get; init; } = new() { Hint = null };
    
    [Required(ErrorMessage = "Enter a password")]
    public string Password { get; init; }    
    
    public string ReturnUrl { get; init; }
    
    public string ForgotPasswordUrl { get; init; }
}