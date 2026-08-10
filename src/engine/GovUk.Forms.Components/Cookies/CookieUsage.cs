namespace GovUk.Forms.Components.Cookies;

public sealed record CookieUsage(string Name, string Purpose, string Lifetime)
{
    public static readonly CookieUsage AntiForgery = new(
        ".AspNetCore.Antiforgery",
        "Per request tampering check",
        "2 hours");
    public static readonly CookieUsage Identity = new(
        ".AspNetCore.Cookies",
        "Authentication cookie for identity",
        "30 minutes");
    public static readonly CookieUsage Affinity = new(
        "ARRAffinity",
        "Session affinity in multi-instance environments",
        "Session");
    public static readonly CookieUsage AffinitySameSite = new(
        "ARRAffinitySameSite",
        "Session affinity for the same instance used",
        "Session");
}