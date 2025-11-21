using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for a repository that manages <see cref="AuthenticationProvider"/> entities.
    /// </summary>
    public interface IAuthenticationProviderRepository : IRepositoryBase<AuthenticationProvider> { }
}