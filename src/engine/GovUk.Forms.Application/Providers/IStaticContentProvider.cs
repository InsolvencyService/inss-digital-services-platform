namespace GovUk.Forms.Application.Providers;

public interface IStaticContentProvider
{
    Task<string> GetAsync(string key);
}