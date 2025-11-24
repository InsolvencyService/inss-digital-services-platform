using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// API controller for managing products.
/// </summary>
[Route("[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="productRepository">The product repository.</param>
    public ProductController(ILogger<ProductController> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Gets a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The product if found; otherwise, a not found or error response.</returns>
    [HttpGet("{productId}")]
    public async Task<IActionResult> Get(Guid productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get product with ID {ProductId}", productId);

        OperationResult<Product> result = await _productRepository.GetByIdAsync(productId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product not found with ID {productId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all products or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all products");

        OperationResult<IEnumerable<Product>> result = await _productRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created product or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create a product with ID {ProductId}", product.Id);

        Helpers.Auditing.SetCreatedInfo(product, ControllerContext);

        OperationResult<Product> result = await _productRepository.AddAsync(product, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { productId = product.Id }, product)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated product or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Product product, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update a product with ID {ProductId}", product.Id);

        Helpers.Auditing.SetModifiedInfo(product, ControllerContext);

        OperationResult<Product> result = await _productRepository.UpdateAsync(product, cancellationToken);

        return result.Success
            ? Ok(product)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{productId}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete product with ID {ProductId}", productId);

        OperationResult<Product> result = await _productRepository.DeleteAsync(productId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Product not found with ID {productId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
