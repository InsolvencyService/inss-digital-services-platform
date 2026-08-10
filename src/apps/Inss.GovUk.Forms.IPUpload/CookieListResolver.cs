using GovUk.Forms.Components.Cookies;

namespace Inss.GovUk.Forms.IPUpload;

public sealed class CookieListResolver : ICookieListResolver
{
    public IEnumerable<CookieUsage> Resolve()
    {
        yield return CookieUsage.AntiForgery;
        yield return CookieUsage.Identity;
        yield return CookieUsage.Affinity;
        yield return CookieUsage.AffinitySameSite;
    }
}