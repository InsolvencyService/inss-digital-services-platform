using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ProductController"/> class.
    /// </summary>
    public class ProductControllerTests
    {
        private readonly Mock<ILogger<ProductController>> _loggerMock = new();
        private readonly Mock<IProductRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productId = Guid.NewGuid();
            OperationResult<Product> operationResult = Operation.Ok(new Product { Id = productId });
            _repositoryMock.Setup(r => r.GetByIdAsync(productId, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(productId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productId = Guid.NewGuid();
            OperationResult<Product> operationResult = Operation.Fail<Product>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(productId, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(productId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Product not found with ID {productId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenProductsExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<Product> products = new() { new Product { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<Product>> operationResult = Operation.Ok<IEnumerable<Product>>(products);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(products);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<Product>> operationResult = Operation.Fail<IEnumerable<Product>>("Failed to get products.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get products.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenProductIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Product product = new() { Id = Guid.NewGuid() };
            OperationResult<Product> operationResult = Operation.Ok(product);
            _repositoryMock.Setup(r => r.AddAsync(product, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(product, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(product);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["productId"].Should().Be(product.Id);
                product.CreatedBy.Should().Be("Creator UserName");
                product.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                product.ModifiedBy.Should().BeNull();
                product.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenProductCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Product product = new() { Id = Guid.NewGuid() };
            OperationResult<Product> operationResult = Operation.Fail<Product>("Failed to create product.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(product, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(product, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create product.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenProductIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Product product = new() { Id = Guid.NewGuid() };
            OperationResult<Product> operationResult = Operation.Ok(product);
            _repositoryMock.Setup(r => r.UpdateAsync(product, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(product, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(product);
                product.ModifiedBy.Should().Be("Modifier UserName");
                product.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenProductUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Product product = new() { Id = Guid.NewGuid() };
            OperationResult<Product> operationResult = Operation.Fail<Product>("Failed to update product.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(product, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(product, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update product.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProductIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productId = Guid.NewGuid();
            OperationResult<Product> operationResult = Operation.Ok(new Product { Id = productId });
            _repositoryMock.Setup(r => r.DeleteAsync(productId, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(productId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productId = Guid.NewGuid();
            OperationResult<Product> operationResult = Operation.Fail<Product>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(productId, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(productId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Product not found with ID {productId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenProductDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productId = Guid.NewGuid();
            OperationResult<Product> operationResult = Operation.Fail<Product>("Failed to delete product.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(productId, cancellationToken)).ReturnsAsync(operationResult);

            ProductController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(productId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete product.");
            }
        }

        private ProductController CreateController(string? userName = null)
        {
            ProductController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}