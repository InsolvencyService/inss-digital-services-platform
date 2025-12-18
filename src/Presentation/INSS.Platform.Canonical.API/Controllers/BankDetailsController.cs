using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Canonical.API.Controllers;

/// <summary>
/// API controller for managing bank details entities.
/// </summary>
[Route("[controller]")]
[ApiController]
public class BankDetailsController : ControllerBase
{
    private readonly ILogger<BankDetailsController> _logger;
    private readonly IBankDetailsRepository _bankDetailsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankDetailsController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="BankDetailsRepository">Repository for BankDetails data access.</param>
    public BankDetailsController(ILogger<BankDetailsController> logger, IBankDetailsRepository BankDetailsRepository)
    {
        _logger = logger;
        _bankDetailsRepository = BankDetailsRepository;
    }

    /// <summary>
    /// Gets bank details by its unique identifier.
    /// </summary>
    /// <param name="bankDetailsId">The unique identifier of the bank details.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The bank details entity if found; otherwise, a not found or error response.</returns>
    [HttpGet("{bankDetailsId}")]
    public async Task<IActionResult> Get(Guid bankDetailsId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get bank details with ID {BankDetailsId}", bankDetailsId);
        
        OperationResult<BankDetails> result = await _bankDetailsRepository.GetByIdAsync(bankDetailsId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Bank details not found with ID {bankDetailsId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all bank details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of bank details entities or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all bank details");
        
        OperationResult<IEnumerable<BankDetails>> result = await _bankDetailsRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates new bank details.
    /// </summary>
    /// <param name="bankDetails">The bank details entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The created bank details entity or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BankDetails bankDetails, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create bank details with ID {BankDetailsId}", bankDetails.Id);
        Helpers.Auditing.SetCreatedInfo(bankDetails, ControllerContext);

        OperationResult<BankDetails> result = await _bankDetailsRepository.AddAsync(bankDetails, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { bankDetailsId = bankDetails.Id }, bankDetails)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates existing bank details.
    /// </summary>
    /// <param name="bankDetails">The bank details entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The updated bank details entity or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BankDetails bankDetails, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update bank details with ID {BankDetailsId}", bankDetails.Id);
        Helpers.Auditing.SetModifiedInfo(bankDetails, ControllerContext);

        OperationResult<BankDetails> result = await _bankDetailsRepository.UpdateAsync(bankDetails, cancellationToken);

        return result.Success
            ? Ok(bankDetails)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes bank details by its unique identifier.
    /// </summary>
    /// <param name="bankDetailsId">The unique identifier of the bank details to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{bankDetailsId}")]
    public async Task<IActionResult> Delete(Guid bankDetailsId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete bank details with ID {BankDetailsId}", bankDetailsId);

        OperationResult<BankDetails> result = await _bankDetailsRepository.DeleteAsync(bankDetailsId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Bank details not found with ID {bankDetailsId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
