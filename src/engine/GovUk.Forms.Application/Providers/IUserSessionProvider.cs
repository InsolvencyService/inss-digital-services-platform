namespace GovUk.Forms.Application.Providers;

public interface IUserSessionProvider
{
    Task<string> ResolveAsync();
}