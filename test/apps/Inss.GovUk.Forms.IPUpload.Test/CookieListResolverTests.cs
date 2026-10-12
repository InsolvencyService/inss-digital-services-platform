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

        Assert.Equal(6, cookieUsages.Length);
        Assert.Equal(CookieUsage.AntiForgery, cookieUsages[0]);
        Assert.Equal(CookieUsage.Identity, cookieUsages[1]);
        Assert.Equal(CookieUsage.CookieC1, cookieUsages[2]);
        Assert.Equal(CookieUsage.CookieC2, cookieUsages[3]);
        Assert.Equal(CookieUsage.Affinity, cookieUsages[4]);
        Assert.Equal(CookieUsage.AffinitySameSite, cookieUsages[5]);
    }
}