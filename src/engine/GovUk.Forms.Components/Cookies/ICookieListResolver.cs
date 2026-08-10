namespace GovUk.Forms.Components.Cookies;

public interface ICookieListResolver
{
    IEnumerable<CookieUsage> Resolve();
}