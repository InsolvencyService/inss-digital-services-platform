using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// API controller for managing resources.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ResourceController : ControllerBase
{
    private readonly ILogger<ResourceController> _logger;
    private readonly IResourceRepository _resourceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="resourceRepository">The resource repository.</param>
    public ResourceController(ILogger<ResourceController> logger, IResourceRepository resourceRepository)
    {
        _logger = logger;
        _resourceRepository = resourceRepository;
    }

    /// <summary>
    /// Gets a resource by its unique identifier.
    /// </summary>
    /// <param name="resourceId">The unique identifier of the resource.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The resource if found; otherwise, a not found or error response.</returns>
    [HttpGet("{resourceId}")]
    public async Task<IActionResult> Get(Guid resourceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get resource with ID {ResourceId}", resourceId);

        OperationResult<Resource> result = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Resource not found with ID {resourceId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all resources.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all resources or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all resources");

        OperationResult<IEnumerable<Resource>> result = await _resourceRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="resource">The resource to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created resource or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Resource resource, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a resource with ID {ResourceId}", resource.Id);

        Helpers.Auditing.SetCreatedInfo(resource, ControllerContext);

        OperationResult<Resource> result = await _resourceRepository.AddAsync(resource, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { resourceId = resource.Id }, resource)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="resource">The resource to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated resource or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Resource resource, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a resource with ID {ResourceId}", resource.Id);

        Helpers.Auditing.SetModifiedInfo(resource, ControllerContext);

        OperationResult<Resource> result = await _resourceRepository.UpdateAsync(resource, cancellationToken);

        return result.Success
            ? Ok(resource)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a resource by its unique identifier.
    /// </summary>
    /// <param name="resourceId">The unique identifier of the resource to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{resourceId}")]
    public async Task<IActionResult> Delete(Guid resourceId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete resource with ID {ResourceId}", resourceId);

        OperationResult<Resource> result = await _resourceRepository.DeleteAsync(resourceId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Resource not found with ID {resourceId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
