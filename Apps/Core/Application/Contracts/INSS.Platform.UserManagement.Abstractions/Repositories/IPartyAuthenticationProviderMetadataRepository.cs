using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for a repository that manages <see cref="PartyAuthenticationProviderMetadata"/> entities.
    /// </summary>
    public interface IPartyAuthenticationProviderMetadataRepository : IRepositoryBase<PartyAuthenticationProviderMetadata> 
    { 
    }
}