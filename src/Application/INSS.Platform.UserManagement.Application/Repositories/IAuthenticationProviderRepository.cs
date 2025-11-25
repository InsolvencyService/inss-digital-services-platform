using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines a contract for a repository that manages <see cref="AuthenticationProvider"/> entities.
/// </summary>
public interface IAuthenticationProviderRepository : IRepositoryBase<AuthenticationProvider> { }