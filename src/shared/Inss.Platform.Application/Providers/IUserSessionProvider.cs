using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Application.Providers;

public interface IUserSessionProvider
{
    Task<(SessionId SessionId, string Email)> ResolveAsync();
}