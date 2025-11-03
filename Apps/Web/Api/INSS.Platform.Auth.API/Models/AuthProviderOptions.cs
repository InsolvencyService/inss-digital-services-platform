using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Auth.API.Models
{
    /// <summary>
    /// Represents authentication provider configuration options.
    /// </summary>
    public class AuthProviderOptions : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the Entra authentication provider options.
        /// </summary>
        public EntraOptions Entra { get; set; } = new();

        /// <summary>
        /// Gets or sets the OneLogin authentication provider options.
        /// </summary>
        public OneLoginOptions OneLogin { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of allowed URIs to redirect to after sign-in.
        /// </summary>
        [Required]
        public List<string> AllowedPostSignInRedirectUris { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of allowed URIs to redirect to after sign-out.
        /// </summary>
        [Required]
        public List<string> AllowedPostSignOutRedirectUris { get; set; } = new();

        /// <summary>
        /// Validates the current <see cref="AuthProviderOptions"/> instance.
        /// </summary>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>A collection of <see cref="ValidationResult"/> objects.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OneLogin.Scopes.Count == 0)
            {
                yield return new ValidationResult("Scopes must not be empty.", [nameof(OneLogin.Scopes)]);
            }

            if (AllowedPostSignInRedirectUris.Count == 0)
            {
                yield return new ValidationResult("AllowedPostSignInRedirectUris must not be empty.", [nameof(AllowedPostSignInRedirectUris)]);
            }

            if (AllowedPostSignOutRedirectUris.Count == 0)
            {
                yield return new ValidationResult("AllowedPostSignOutRedirectUris must not be empty.", [nameof(AllowedPostSignOutRedirectUris)]);
            }
        }
    }
}
