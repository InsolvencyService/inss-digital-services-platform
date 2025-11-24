using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing user groups.
/// </summary>
[Route("[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly IGroupRepository _groupRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="groupRepository">Repository for group data access.</param>
    public GroupController(ILogger<GroupController> logger, IGroupRepository groupRepository)
    {
        _logger = logger;
        _groupRepository = groupRepository;
    }

    /// <summary>
    /// Gets a group by its unique identifier.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The group if found; otherwise, a NotFound or error response.</returns>
    [HttpGet("{groupId}")]
    public async Task<IActionResult> Get(Guid groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get group with ID {GroupId}", groupId);

        OperationResult<Group> result = await _groupRepository.GetByIdAsync(groupId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Group not found with ID {groupId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all groups.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A list of all groups or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all groups");

        OperationResult<IEnumerable<Group>> result = await _groupRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="group">The group entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The created group or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Group group, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a group with ID {GroupId}", group.Id);

        Helpers.Auditing.SetCreatedInfo(group, ControllerContext);

        OperationResult<Group> result = await _groupRepository.AddAsync(group, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { groupId = group.Id }, group)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="group">The group entity with updated information.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The updated group or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Group group, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a group with ID {GroupId}", group.Id);

        Helpers.Auditing.SetModifiedInfo(group, ControllerContext);

        OperationResult<Group> result = await _groupRepository.UpdateAsync(group, cancellationToken);

        return result.Success
            ? Ok(group)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a group by its unique identifier.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>No content if successful; otherwise, a NotFound or error response.</returns>
    [HttpDelete("{groupId}")]
    public async Task<IActionResult> Delete(Guid groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete group with ID {GroupId}", groupId);

        OperationResult<Group> result = await _groupRepository.DeleteAsync(groupId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Group not found with ID {groupId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
