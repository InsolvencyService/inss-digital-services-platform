using GovUk.Forms.Application.Providers;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class TestUserSessionProvider : IUserSessionProvider
{
    public Task<string> ResolveAsync()
    {
        return Task.FromResult("BF90BE8F-521F-4450-940E-52FEAF42D96C");
    }
}