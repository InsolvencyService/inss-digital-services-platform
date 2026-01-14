using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Canonical.API.Controllers;

/// <summary>
/// API controller for managing income entities.
/// </summary>
[Route("[controller]")]
[ApiController]
public class IncomeController : ControllerBase
{
    private readonly ILogger<IncomeController> _logger;
    private readonly IIncomeRepository _incomeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomeController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="IncomeRepository">Repository for Income data access.</param>
    public IncomeController(ILogger<IncomeController> logger, IIncomeRepository IncomeRepository)
    {
        _logger = logger;
        _incomeRepository = IncomeRepository;
    }

    /// <summary>
    /// Gets an income by its unique identifier.
    /// </summary>
    /// <param name="incomeId">The unique identifier of the income.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The income entity if found; otherwise, a not found or error response.</returns>
    [HttpGet("{incomeId}")]
    public async Task<IActionResult> Get(Guid incomeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get income with ID {IncomeId}", incomeId);
        
        OperationResult<Income> result = await _incomeRepository.GetByIdAsync(incomeId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Income not found with ID {incomeId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all incomes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of income entities or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all incomes");
        
        OperationResult<IEnumerable<Income>> result = await _incomeRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new income.
    /// </summary>
    /// <param name="income">The income entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The created income entity or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Income income, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an income with ID {IncomeId}", income.Id);

        Helpers.Auditing.SetCreatedInfo(income, ControllerContext);

        OperationResult<Income> result = await _incomeRepository.AddAsync(income, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { incomeId = income.Id }, income)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing income.
    /// </summary>
    /// <param name="income">The income entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The updated income entity or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Income income, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an income with ID {IncomeId}", income.Id);

        Helpers.Auditing.SetModifiedInfo(income, ControllerContext);

        OperationResult<Income> result = await _incomeRepository.UpdateAsync(income, cancellationToken);

        return result.Success
            ? Ok(income)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an income by its unique identifier.
    /// </summary>
    /// <param name="incomeId">The unique identifier of the income to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{incomeId}")]
    public async Task<IActionResult> Delete(Guid incomeId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete income with ID {IncomeId}", incomeId);

        OperationResult<Income> result = await _incomeRepository.DeleteAsync(incomeId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Income not found with ID {incomeId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
