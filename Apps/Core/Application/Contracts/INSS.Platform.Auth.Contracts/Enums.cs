namespace INSS.Platform.Auth.Contracts
{
    /// <summary>
    /// Specifies the available authentication providers.
    /// </summary>
    public enum AuthenticationProvider
    {
        /// <summary>
        /// The OneLogin authentication provider.
        /// </summary>
        OneLogin,

        /// <summary>
        /// The Entra (Microsoft Entra ID) authentication provider.
        /// </summary>
        Entra
    }
}
