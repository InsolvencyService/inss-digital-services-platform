using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines a contract for a repository that manages <see cref="AuthenticationProviderMetadata"/> entities.
/// </summary>
public interface IAuthenticationProviderMetadataRepository : IRepositoryBase<AuthenticationProviderMetadata> { }