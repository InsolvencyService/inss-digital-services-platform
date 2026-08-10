using GovUk.Forms.Components.Cookies;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test;

public class CookieListResolverTests
{
    [Fact]
    public void ListedCookies_Resolve_ReturnsExpectedAppCookies()
    {
        CookieListResolver cookieListResolver = new();

        CookieUsage[] cookieUsages = cookieListResolver.Resolve().ToArray();
        
        Assert.Equal(4, cookieUsages.Length);
        Assert.Equal(CookieUsage.AntiForgery, cookieUsages[0]);
        Assert.Equal(CookieUsage.Identity, cookieUsages[1]);
        Assert.Equal(CookieUsage.Affinity, cookieUsages[2]);
        Assert.Equal(CookieUsage.AffinitySameSite, cookieUsages[3]);
    }
}