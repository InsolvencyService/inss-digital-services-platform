using Inss.Platform.Application.Providers;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Infrastructure.Providers;

public sealed class NoopUserSessionProvider : IUserSessionProvider
{
    private static readonly SessionId _noopSessionId = "ad624640d0ea4710959ec309354db83a";
    
    public Task<(SessionId SessionId, string Email)> ResolveAsync()
    {
        // This is used by apps such as FIP which do not require any session state to be stored
        return Task.FromResult((_noopSessionId, string.Empty));
    }
}