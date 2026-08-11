using GovUk.Forms.Components.Cookies;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test;

public class CookieListResolverTests
{
    [Fact]
    public void ListedCookies_Resolve_ReturnsExpectedAppCookies()
    {
        CookieListResolver cookieListResolver = new();

        CookieUsage[] cookieUsages = cookieListResolver.Resolve().ToArray();
        
        Assert.Equal(3, cookieUsages.Length);
        Assert.Equal(CookieUsage.AntiForgery, cookieUsages[0]);
        Assert.Equal(CookieUsage.Affinity, cookieUsages[1]);
        Assert.Equal(CookieUsage.AffinitySameSite, cookieUsages[2]);
    }
}