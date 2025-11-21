using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for repository operations related to <see cref="AuthenticationPolicy"/> entities.
    /// </summary>
    public interface IAuthenticationPolicyRepository : IRepositoryBase<AuthenticationPolicy> { }
}