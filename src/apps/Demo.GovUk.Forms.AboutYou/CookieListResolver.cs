using GovUk.Forms.Components.Cookies;

namespace Demo.GovUk.Forms.AboutYou;

public sealed class CookieListResolver : ICookieListResolver
{
    public IEnumerable<CookieUsage> Resolve()
    {
        yield return CookieUsage.AntiForgery;
        yield return CookieUsage.Affinity;
        yield return CookieUsage.AffinitySameSite;
    }
}