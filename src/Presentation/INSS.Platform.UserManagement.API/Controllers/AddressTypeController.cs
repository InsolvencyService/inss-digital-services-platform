using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// API controller for managing address types.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AddressTypeController : ControllerBase
{
    private readonly ILogger<AddressTypeController> _logger;
    private readonly IAddressTypeRepository _addressTypeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressTypeController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="addressTypeRepository">The address type repository.</param>
    public AddressTypeController(ILogger<AddressTypeController> logger, IAddressTypeRepository addressTypeRepository)
    {
        _logger = logger;
        _addressTypeRepository = addressTypeRepository;
    }

    /// <summary>
    /// Gets an address type by its unique identifier.
    /// </summary>
    /// <param name="addressTypeId">The unique identifier of the address type.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The address type if found; otherwise, a not found or error response.</returns>
    [HttpGet("{addressTypeId}")]
    public async Task<IActionResult> Get(Guid addressTypeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get address type with ID {AddressId}", addressTypeId);
        
        OperationResult<AddressType> result = await _addressTypeRepository.GetByIdAsync(addressTypeId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Address type not found with ID {addressTypeId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all address types.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all address types or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all address types");
        
        OperationResult<IEnumerable<AddressType>> result = await _addressTypeRepository.GetAsync(cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new address type.
    /// </summary>
    /// <param name="addressType">The address type to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created address type or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddressType addressType, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an address type with ID {AddressTypeId}", addressType.Id);

        Helpers.Auditing.SetCreatedInfo(addressType, ControllerContext);

        OperationResult<AddressType> result = await _addressTypeRepository.AddAsync(addressType, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { addressTypeId = addressType.Id }, addressType)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing address type.
    /// </summary>
    /// <param name="addressType">The address type to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated address type or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AddressType addressType, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an address type with ID {AddressTypeId}", addressType.Id);

        Helpers.Auditing.SetModifiedInfo(addressType, ControllerContext);

        OperationResult<AddressType> result = await _addressTypeRepository.UpdateAsync(addressType, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(addressType)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an address type by its unique identifier.
    /// </summary>
    /// <param name="addressTypeId">The unique identifier of the address type to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{addressTypeId}")]
    public async Task<IActionResult> Delete(Guid addressTypeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete address type with ID {AddressTypeId}", addressTypeId);

        OperationResult<AddressType> result = await _addressTypeRepository.DeleteAsync(addressTypeId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Address type not found with ID {addressTypeId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
