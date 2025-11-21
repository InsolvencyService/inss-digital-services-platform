using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing product role resource permissions.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ProductRoleResourcePermissionController : ControllerBase
    {
        private readonly ILogger<ProductRoleResourcePermissionController> _logger;
        private readonly IProductRoleResourcePermissionRepository _productRoleResourcePermissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRoleResourcePermissionController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="productRoleResourcePermissionRepository">The repository for product role resource permissions.</param>
        public ProductRoleResourcePermissionController(
            ILogger<ProductRoleResourcePermissionController> logger,
            IProductRoleResourcePermissionRepository productRoleResourcePermissionRepository)
        {
            _logger = logger;
            _productRoleResourcePermissionRepository = productRoleResourcePermissionRepository;
        }

        /// <summary>
        /// Gets a product role resource permission by its unique identifier.
        /// </summary>
        /// <param name="productRoleResourcePermissionId">The unique identifier of the product role resource permission.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The product role resource permission if found; otherwise, a not found or error response.</returns>
        [HttpGet("{productRoleResourcePermissionId}")]
        public async Task<IActionResult> Get(Guid productRoleResourcePermissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get product role resource permission with ID {ProductRoleResourcePermissionId}", productRoleResourcePermissionId);

            OperationResult<ProductRoleResourcePermission> result = await _productRoleResourcePermissionRepository.GetByIdAsync(productRoleResourcePermissionId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Product role resource permission not found with ID {productRoleResourcePermissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all product role resource permissions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all product role resource permissions or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all product role resource permissions");

            OperationResult<IEnumerable<ProductRoleResourcePermission>> result = await _productRoleResourcePermissionRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new product role resource permission.
        /// </summary>
        /// <param name="productRoleResourcePermission">The product role resource permission to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created product role resource permission or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRoleResourcePermission productRoleResourcePermission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a product role resource permission with ID {ProductRoleResourcePermissionId}", productRoleResourcePermission.Id);

            Helpers.Auditing.SetCreatedInfo(productRoleResourcePermission, ControllerContext);

            OperationResult<ProductRoleResourcePermission> result = await _productRoleResourcePermissionRepository.AddAsync(productRoleResourcePermission, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { productRoleResourcePermissionId = productRoleResourcePermission.Id }, productRoleResourcePermission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing product role resource permission.
        /// </summary>
        /// <param name="productRoleResourcePermission">The product role resource permission to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated product role resource permission or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductRoleResourcePermission productRoleResourcePermission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a product role resource permission with ID {ProductRoleResourcePermissionId}", productRoleResourcePermission.Id);

            Helpers.Auditing.SetModifiedInfo(productRoleResourcePermission, ControllerContext);

            OperationResult<ProductRoleResourcePermission> result = await _productRoleResourcePermissionRepository.UpdateAsync(productRoleResourcePermission, cancellationToken);

            return result.Success
                ? Ok(productRoleResourcePermission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a product role resource permission by its unique identifier.
        /// </summary>
        /// <param name="productRoleResourcePermissionId">The unique identifier of the product role resource permission to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{productRoleResourcePermissionId}")]
        public async Task<IActionResult> Delete(Guid productRoleResourcePermissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete product role resource permission with ID {ProductRoleResourcePermissionId}", productRoleResourcePermissionId);

            OperationResult<ProductRoleResourcePermission> result = await _productRoleResourcePermissionRepository.DeleteAsync(productRoleResourcePermissionId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Product role resource permission not found with ID {productRoleResourcePermissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
