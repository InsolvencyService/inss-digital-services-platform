using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines a contract for a repository that manages <see cref="AuthenticationPolicyProvider"/> entities.
/// </summary>
public interface IAuthenticationPolicyProviderRepository : IRepositoryBase<AuthenticationPolicyProvider> { }