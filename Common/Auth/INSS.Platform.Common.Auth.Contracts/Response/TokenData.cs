namespace INSS.Platform.Common.Auth.Contracts.Response
{
    /// <summary>
    /// Data transfer object that represents token information returned from an authentication provider.
    /// </summary>
    /// <remarks>
    /// This DTO contains the tokens and timing values required to authenticate API requests
    /// and to determine token validity. Values are simple primitives to keep serialization
    /// straightforward between services.
    /// </remarks>
    public class TokenData
    {
        /// <summary>
        /// Gets or sets the access token used for API authentication.
        /// </summary>
        /// <value>
        /// The OAuth2 access token (a JWT) used to authorize API calls.
        /// </value>
        /// <example>eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID token containing user identity claims.
        /// </summary>
        /// <value>
        /// The OpenID Connect ID token (a JWT) that contains claims about the authenticated user.
        /// </value>
        /// <example>eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9...</example>
        public string IdToken { get; set; } = string.Empty;
    }
}
