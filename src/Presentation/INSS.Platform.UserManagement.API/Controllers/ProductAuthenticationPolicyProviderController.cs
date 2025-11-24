using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing product authentication policy providers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ProductAuthenticationPolicyProviderController : ControllerBase
{
    private readonly ILogger<ProductAuthenticationPolicyProviderController> _logger;
    private readonly IProductAuthenticationPolicyProviderRepository _productAuthenticationPolicyProviderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductAuthenticationPolicyProviderController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="productAuthenticationPolicyProviderRepository">The repository for product authentication policy providers.</param>
    public ProductAuthenticationPolicyProviderController(
        ILogger<ProductAuthenticationPolicyProviderController> logger,
        IProductAuthenticationPolicyProviderRepository productAuthenticationPolicyProviderRepository)
    {
        _logger = logger;
        _productAuthenticationPolicyProviderRepository = productAuthenticationPolicyProviderRepository;
    }

    /// <summary>
    /// Gets a product authentication policy provider by its unique identifier.
    /// </summary>
    /// <param name="productAuthenticationPolicyProviderId">The unique identifier of the product authentication policy provider.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The product authentication policy provider if found; otherwise, a not found or error response.</returns>
    [HttpGet("{productAuthenticationPolicyProviderId}")]
    public async Task<IActionResult> Get(Guid productAuthenticationPolicyProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get product authentication policy provider with ID {ProductAuthenticationPolicyProviderId}", productAuthenticationPolicyProviderId);

        OperationResult<ProductAuthenticationPolicyProvider> result = await _productAuthenticationPolicyProviderRepository.GetByIdAsync(productAuthenticationPolicyProviderId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product authentication policy provider not found with ID {productAuthenticationPolicyProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all product authentication policy providers.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all product authentication policy providers or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all product authentication policy providers");

        OperationResult<IEnumerable<ProductAuthenticationPolicyProvider>> result = await _productAuthenticationPolicyProviderRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new product authentication policy provider.
    /// </summary>
    /// <param name="productAuthenticationPolicyProvider">The product authentication policy provider to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created product authentication policy provider or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductAuthenticationPolicyProvider productAuthenticationPolicyProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a product authentication policy provider with ID {ProductAuthenticationPolicyProviderId}", productAuthenticationPolicyProvider.Id);

        Helpers.Auditing.SetCreatedInfo(productAuthenticationPolicyProvider, ControllerContext);

        OperationResult<ProductAuthenticationPolicyProvider> result = await _productAuthenticationPolicyProviderRepository.AddAsync(productAuthenticationPolicyProvider, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { productAuthenticationPolicyProviderId = productAuthenticationPolicyProvider.Id }, productAuthenticationPolicyProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing product authentication policy provider.
    /// </summary>
    /// <param name="productAuthenticationPolicyProvider">The product authentication policy provider to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated product authentication policy provider or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductAuthenticationPolicyProvider productAuthenticationPolicyProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a product authentication policy provider with ID {ProductAuthenticationPolicyProviderId}", productAuthenticationPolicyProvider.Id);

        Helpers.Auditing.SetModifiedInfo(productAuthenticationPolicyProvider, ControllerContext);

        OperationResult<ProductAuthenticationPolicyProvider> result = await _productAuthenticationPolicyProviderRepository.UpdateAsync(productAuthenticationPolicyProvider, cancellationToken);

        return result.Success
            ? Ok(productAuthenticationPolicyProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a product authentication policy provider by its unique identifier.
    /// </summary>
    /// <param name="productAuthenticationPolicyProviderId">The unique identifier of the product authentication policy provider to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{productAuthenticationPolicyProviderId}")]
    public async Task<IActionResult> Delete(Guid productAuthenticationPolicyProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete product authentication policy provider with ID {ProductAuthenticationPolicyProviderId}", productAuthenticationPolicyProviderId);

        OperationResult<ProductAuthenticationPolicyProvider> result = await _productAuthenticationPolicyProviderRepository.DeleteAsync(productAuthenticationPolicyProviderId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product authentication policy provider not found with ID {productAuthenticationPolicyProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
