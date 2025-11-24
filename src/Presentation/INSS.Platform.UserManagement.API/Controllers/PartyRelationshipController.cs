using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing party relationships.
/// </summary>
[Route("[controller]")]
[ApiController]
public class PartyRelationshipController : ControllerBase
{
    private readonly ILogger<PartyRelationshipController> _logger;
    private readonly IPartyRelationshipRepository _partyRelationshipRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyRelationshipController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="partyRelationshipRepository">The party relationship repository.</param>
    public PartyRelationshipController(ILogger<PartyRelationshipController> logger, IPartyRelationshipRepository partyRelationshipRepository)
    {
        _logger = logger;
        _partyRelationshipRepository = partyRelationshipRepository;
    }

    /// <summary>
    /// Gets a party relationship by its unique identifier.
    /// </summary>
    /// <param name="partyRelationshipId">The unique identifier of the party relationship.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The party relationship if found; otherwise, a not found or error response.</returns>
    [HttpGet("{partyRelationshipId}")]
    public async Task<IActionResult> Get(Guid partyRelationshipId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get party relationship with ID {PartyRelationshipId}", partyRelationshipId);

        OperationResult<PartyRelationship> result = await _partyRelationshipRepository.GetByIdAsync(partyRelationshipId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Party relationship not found with ID {partyRelationshipId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all party relationships.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all party relationships or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all party relationships");

        OperationResult<IEnumerable<PartyRelationship>> result = await _partyRelationshipRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new party relationship.
    /// </summary>
    /// <param name="partyRelationship">The party relationship to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created party relationship or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PartyRelationship partyRelationship, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a party relationship with ID {PartyRelationshipId}", partyRelationship.Id);

        Helpers.Auditing.SetCreatedInfo(partyRelationship, ControllerContext);

        OperationResult<PartyRelationship> result = await _partyRelationshipRepository.AddAsync(partyRelationship, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { partyRelationshipId = partyRelationship.Id }, partyRelationship)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing party relationship.
    /// </summary>
    /// <param name="partyRelationship">The party relationship to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated party relationship or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PartyRelationship partyRelationship, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a party relationship with ID {PartyRelationshipId}", partyRelationship.Id);

        Helpers.Auditing.SetModifiedInfo(partyRelationship, ControllerContext);

        OperationResult<PartyRelationship> result = await _partyRelationshipRepository.UpdateAsync(partyRelationship, cancellationToken);

        return result.Success
            ? Ok(partyRelationship)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a party relationship by its unique identifier.
    /// </summary>
    /// <param name="partyRelationshipId">The unique identifier of the party relationship to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{partyRelationshipId}")]
    public async Task<IActionResult> Delete(Guid partyRelationshipId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete party relationship with ID {PartyRelationshipId}", partyRelationshipId);

        OperationResult<PartyRelationship> result = await _partyRelationshipRepository.DeleteAsync(partyRelationshipId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Party relationship not found with ID {partyRelationshipId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
