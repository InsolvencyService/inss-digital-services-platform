using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Abstractions.Services;
using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Repository.Services
{
    /// <summary>
    /// Provides lookup services for <see cref="RelationshipType"/> entities.
    /// </summary>
    public class RelationshipTypeLookupService : IRelationshipTypeLookupService
    {
        private readonly IRelationshipTypeRepository _relationshipTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipTypeLookupService"/> class.
        /// </summary>
        /// <param name="relationshipTypeRepository">The repository used to access relationship type data.</param>
        public RelationshipTypeLookupService(IRelationshipTypeRepository relationshipTypeRepository)
        {
            _relationshipTypeRepository = relationshipTypeRepository;
        }

        /// <summary>
        /// Retrieves a <see cref="RelationshipType"/> entity by its name asynchronously.
        /// </summary>
        /// <param name="name">The name of the relationship type to retrieve.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation.
        /// The task result contains the <see cref="RelationshipType"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<RelationshipType?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            OperationResult<RelationshipType> result = await _relationshipTypeRepository.GetByNameAsync(name, cancellationToken);

            return result.Entity;
        }
    }
}
