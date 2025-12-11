using System.Security.Claims;

namespace INSS.Platform.Shared.Web.Auth.Services;

/// <summary>
/// Provides methods for authentication-related operations.
/// </summary>
public interface IJwtAuthenticationService
{
    /// <summary>
    /// Validates the specified JSON Web Token (JWT) and extracts the associated <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="jwt">The JWT string to validate.</param>
    /// <param name="principal">
    /// When this method returns, contains the <see cref="ClaimsPrincipal"/> extracted from the JWT if validation is successful; otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the JWT is valid and the principal was successfully extracted; otherwise, <c>false</c>.
    /// </returns>
    bool ValidateJwt(string jwt, out ClaimsPrincipal principal);
}
