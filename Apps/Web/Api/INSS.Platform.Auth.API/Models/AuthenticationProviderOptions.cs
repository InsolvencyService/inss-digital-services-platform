using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Models
{
    /// <summary>
    /// Represents authentication provider configuration options.
    /// </summary>
    public class AuthenticationProviderOptions : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the configuration options for the INSS JWT.
        /// </summary>
        public InssOptions Inss { get; set; } = new();

        /// <summary>
        /// Gets or sets the Entra authentication provider options.
        /// </summary>
        public EntraOptions Entra { get; set; } = new();

        /// <summary>
        /// Gets or sets the OneLogin authentication provider options.
        /// </summary>
        public OneLoginOptions OneLogin { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of allowed client URLs for redirect after after sign-in/out.
        /// </summary>
        [Required]
        public List<string> AllowedClientRedirectUrls { get; set; } = [];

        /// <summary>
        /// Validates the current <see cref="AuthenticationProviderOptions"/> instance.
        /// </summary>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>A collection of <see cref="ValidationResult"/> objects.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Entra.SignOutCallbackPath.StartsWith('/'))
            {
                yield return new ValidationResult("Entra SignOutCallbackPath must not start with a '/'.", [nameof(Entra.SignOutCallbackPath)]);
            }

            if (!Entra.SignInCallbackPath.StartsWith('/'))
            {
                yield return new ValidationResult("Entra SignInCallbackPath must start with a '/'.", [nameof(Entra.SignInCallbackPath)]);
            }

            if (Entra.Scopes.Count == 0)
            {
                yield return new ValidationResult("Entra Scopes must not be empty.", [nameof(Entra.Scopes)]);
            }

            if (OneLogin.SignOutCallbackPath.StartsWith('/'))
            {
                yield return new ValidationResult("OneLogin SignOutCallbackPath must not start with a '/'.", [nameof(OneLogin.SignOutCallbackPath)]);
            }

            if (! OneLogin.SignInCallbackPath.StartsWith('/'))
            {
                yield return new ValidationResult("OneLogin SignInCallbackPath must start with a '/'.", [nameof(OneLogin.SignInCallbackPath)]);
            }

            if (OneLogin.Scopes.Count == 0)
            {
                yield return new ValidationResult("OneLoginScopes must not be empty.", [nameof(OneLogin.Scopes)]);
            }

            if (AllowedClientRedirectUrls.Count == 0)
            {
                yield return new ValidationResult("AllowedClientRedirectUrls must not be empty.", [nameof(AllowedClientRedirectUrls)]);
            }
        }
    }
}
