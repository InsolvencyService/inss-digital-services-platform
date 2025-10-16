namespace INSS.Platform.Auth.API.Dto
{
    /// <summary>
    /// Represents token information returned from an authentication provider.
    /// </summary>
    public class TokenData
    {
        /// <summary>
        /// Gets or sets the access token used for API authentication.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID token containing user identity claims.
        /// </summary>
        public string IdToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of seconds until the token expires.
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
