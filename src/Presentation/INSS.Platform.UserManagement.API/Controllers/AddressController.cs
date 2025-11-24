using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// API controller for managing address entities.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly ILogger<AddressController> _logger;
    private readonly IAddressRepository _addressRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="addressRepository">Repository for address data access.</param>
    public AddressController(ILogger<AddressController> logger, IAddressRepository addressRepository)
    {
        _logger = logger;
        _addressRepository = addressRepository;
    }

    /// <summary>
    /// Gets an address by its unique identifier.
    /// </summary>
    /// <param name="addressId">The unique identifier of the address.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The address entity if found; otherwise, a not found or error response.</returns>
    [HttpGet("{addressId}")]
    public async Task<IActionResult> Get(Guid addressId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get address with ID {AddressId}", addressId);
        
        OperationResult<Address> result = await _addressRepository.GetByIdAsync(addressId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Address not found with ID {addressId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all addresses.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of address entities or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all addresses");
        
        OperationResult<IEnumerable<Address>> result = await _addressRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new address.
    /// </summary>
    /// <param name="address">The address entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The created address entity or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Address address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an address with ID {AddressId}", address.Id);

        Helpers.Auditing.SetCreatedInfo(address, ControllerContext);

        OperationResult<Address> result = await _addressRepository.AddAsync(address, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { addressId = address.Id }, address)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing address.
    /// </summary>
    /// <param name="address">The address entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The updated address entity or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Address address, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an address with ID {AddressId}", address.Id);

        Helpers.Auditing.SetModifiedInfo(address, ControllerContext);

        OperationResult<Address> result = await _addressRepository.UpdateAsync(address, cancellationToken);

        return result.Success
            ? Ok(address)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an address by its unique identifier.
    /// </summary>
    /// <param name="addressId">The unique identifier of the address to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{addressId}")]
    public async Task<IActionResult> Delete(Guid addressId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete address with ID {AddressId}", addressId);

        OperationResult<Address> result = await _addressRepository.DeleteAsync(addressId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Address not found with ID {addressId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
