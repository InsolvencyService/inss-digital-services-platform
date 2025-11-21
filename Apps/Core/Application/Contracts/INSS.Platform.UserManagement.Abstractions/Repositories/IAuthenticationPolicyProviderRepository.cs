using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for a repository that manages <see cref="AuthenticationPolicyProvider"/> entities.
    /// </summary>
    public interface IAuthenticationPolicyProviderRepository : IRepositoryBase<AuthenticationPolicyProvider> { }
}