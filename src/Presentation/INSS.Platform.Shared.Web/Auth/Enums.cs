using System.ComponentModel;

namespace INSS.Platform.Shared.Web.Auth;
/// <summary>
/// Specifies the available authentication providers.
/// </summary>
public enum AuthenticationProvider
{
    /// <summary>
    /// The OneLogin authentication provider.
    /// </summary>
    [Description("GOV.UK One Login")]
    OneLogin,

    /// <summary>
    /// The Entra (Microsoft Entra ID) authentication provider.
    /// </summary>
    [Description("Microsoft Entra ID")]
    Entra
}
