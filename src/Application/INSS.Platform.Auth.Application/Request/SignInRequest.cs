using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace INSS.Platform.Auth.Application.Request;

/// <summary>
/// Represents a request to sign in a user, including client redirect information.
/// </summary>
[ExcludeFromCodeCoverage(Justification ="DTO's, there is no business logic to test.")]
public class SignInRequest
{
    /// <summary>
    /// Gets or sets the client redirect URL.
    /// </summary>
    [Required]
    public string ClientRedirectUrl { get; set; }

    /// <summary>
    /// Gets or sets the optional user identifier for the sign-in request.
    /// </summary>
    public string? UserId { get; set; }
}
