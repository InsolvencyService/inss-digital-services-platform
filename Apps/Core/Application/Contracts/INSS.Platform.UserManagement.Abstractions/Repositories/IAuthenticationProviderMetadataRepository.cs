using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for a repository that manages <see cref="AuthenticationProviderMetadata"/> entities.
    /// </summary>
    public interface IAuthenticationProviderMetadataRepository : IRepositoryBase<AuthenticationProviderMetadata> 
    { 
    }
}