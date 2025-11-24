using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing product roles.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ProductRoleController : ControllerBase
{
    private readonly ILogger<ProductRoleController> _logger;
    private readonly IProductRoleRepository _productRoleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRoleController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="productRoleRepository">The product role repository.</param>
    public ProductRoleController(ILogger<ProductRoleController> logger, IProductRoleRepository productRoleRepository)
    {
        _logger = logger;
        _productRoleRepository = productRoleRepository;
    }

    /// <summary>
    /// Gets a product role by its unique identifier.
    /// </summary>
    /// <param name="productRoleId">The unique identifier of the product role.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The product role if found; otherwise, a not found or error response.</returns>
    [HttpGet("{productRoleId}")]
    public async Task<IActionResult> Get(Guid productRoleId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get product role with ID {ProductRoleId}", productRoleId);

        OperationResult<ProductRole> result = await _productRoleRepository.GetByIdAsync(productRoleId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product role not found with ID {productRoleId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all product roles.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all product roles or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all product roles");

        OperationResult<IEnumerable<ProductRole>> result = await _productRoleRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new product role.
    /// </summary>
    /// <param name="productRole">The product role to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created product role or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRole productRole, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a product role with ID {ProductRoleId}", productRole.Id);

        Helpers.Auditing.SetCreatedInfo(productRole, ControllerContext);

        OperationResult<ProductRole> result = await _productRoleRepository.AddAsync(productRole, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { productRoleId = productRole.Id }, productRole)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing product role.
    /// </summary>
    /// <param name="productRole">The product role to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated product role or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductRole productRole, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a product role with ID {ProductRoleId}", productRole.Id);

        Helpers.Auditing.SetModifiedInfo(productRole, ControllerContext);

        OperationResult<ProductRole> result = await _productRoleRepository.UpdateAsync(productRole, cancellationToken);

        return result.Success
            ? Ok(productRole)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a product role by its unique identifier.
    /// </summary>
    /// <param name="productRoleId">The unique identifier of the product role to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{productRoleId}")]
    public async Task<IActionResult> Delete(Guid productRoleId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete product role with ID {ProductRoleId}", productRoleId);

        OperationResult<ProductRole> result = await _productRoleRepository.DeleteAsync(productRoleId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product role not found with ID {productRoleId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
