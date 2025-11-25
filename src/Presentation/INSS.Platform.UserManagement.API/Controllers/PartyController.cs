using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// API controller for managing Party entities.
/// </summary>
[Route("[controller]")]
[ApiController]
public class PartyController : ControllerBase
{
    private readonly ILogger<PartyController> _logger;
    private readonly IPartyRepository _partyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="partyRepository">The party repository instance.</param>
    public PartyController(ILogger<PartyController> logger, IPartyRepository partyRepository)
    {
        _logger = logger;
        _partyRepository = partyRepository;
    }

    /// <summary>
    /// Retrieves a party by its unique identifier.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The party entity if found; otherwise, a not found or error response.</returns>
    [HttpGet("{partyId}")]
    public async Task<IActionResult> Get(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get party with ID {PartyId}", partyId);

        OperationResult<Party> result = await _partyRepository.GetByIdAsync(partyId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Party not found with ID {partyId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Retrieves all parties.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all party entities or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all parties");

        OperationResult<IEnumerable<Party>> result = await _partyRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new party.
    /// </summary>
    /// <param name="party">The party entity to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created party entity or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Party party, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a party with ID {PartyId}", party.Id);

        Helpers.Auditing.SetCreatedInfo(party, ControllerContext);

        OperationResult<Party> result = await _partyRepository.AddAsync(party, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { partyId = party.Id }, party)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing party.
    /// </summary>
    /// <param name="party">The party entity to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated party entity or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Party party, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a party with ID {PartyId}", party.Id);

        Helpers.Auditing.SetModifiedInfo(party, ControllerContext);

        OperationResult<Party> result = await _partyRepository.UpdateAsync(party, cancellationToken);

        return result.Success
            ? Ok(party)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a party by its unique identifier.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{partyId}")]
    public async Task<IActionResult> Delete(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete party with ID {PartyId}", partyId);

        OperationResult<Party> result = await _partyRepository.DeleteAsync(partyId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Party not found with ID {partyId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
