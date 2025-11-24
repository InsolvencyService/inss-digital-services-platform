using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.UserManagement.API.Controllers;
using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace INSS.Platform.UserManagement.API.Tests;

/// <summary>
/// Unit tests for the <see cref="ProductRoleController"/> class.
/// </summary>
public class ProductRoleControllerTests
{
    private readonly Mock<ILogger<ProductRoleController>> _loggerMock = new();
    private readonly Mock<IProductRoleRepository> _productRoleRepositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductRoleExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid productRoleId = Guid.NewGuid();
        OperationResult<ProductRole> operationResult = Operation.Ok(new ProductRole { Id = productRoleId });
        _productRoleRepositoryMock.Setup(r => r.GetByIdAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(productRoleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductRoleDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid productRoleId = Guid.NewGuid();
        OperationResult<ProductRole> operationResult = Operation.Fail<ProductRole>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _productRoleRepositoryMock.Setup(r => r.GetByIdAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(productRoleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Product role not found with ID {productRoleId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenProductRolesExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<ProductRole> productRoles = new() { new ProductRole { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<ProductRole>> operationResult = Operation.Ok<IEnumerable<ProductRole>>(productRoles);
        _productRoleRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(productRoles);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<ProductRole>> operationResult = Operation.Fail<IEnumerable<ProductRole>>("Failed to get product roles.", OperationErrorCode.SqlError);
        _productRoleRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get product roles.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenProductRoleIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductRole productRole = new() { Id = Guid.NewGuid() };
        OperationResult<ProductRole> operationResult = Operation.Ok(productRole);
        _productRoleRepositoryMock.Setup(r => r.AddAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(productRole, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(productRole);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["productRoleId"].Should().Be(productRole.Id);
            productRole.CreatedBy.Should().Be("Creator UserName");
            productRole.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            productRole.ModifiedBy.Should().BeNull();
            productRole.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenProductRoleCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductRole productRole = new() { Id = Guid.NewGuid() };
        OperationResult<ProductRole> operationResult = Operation.Fail<ProductRole>("Failed to create product role.", OperationErrorCode.SqlError);
        _productRoleRepositoryMock.Setup(r => r.AddAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(productRole, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create product role.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenProductRoleIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductRole productRole = new() { Id = Guid.NewGuid() };
        OperationResult<ProductRole> operationResult = Operation.Ok(productRole);
        _productRoleRepositoryMock.Setup(r => r.UpdateAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(productRole, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(productRole);
            productRole.ModifiedBy.Should().Be("Modifier UserName");
            productRole.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenProductRoleUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductRole productRole = new() { Id = Guid.NewGuid() };
        OperationResult<ProductRole> operationResult = Operation.Fail<ProductRole>("Failed to update product role.", OperationErrorCode.SqlError);
        _productRoleRepositoryMock.Setup(r => r.UpdateAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(productRole, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update product role.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductRoleIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid productRoleId = Guid.NewGuid();
        OperationResult<ProductRole> operationResult = Operation.Ok(new ProductRole { Id = productRoleId });
        _productRoleRepositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(productRoleId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductRoleDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid productRoleId = Guid.NewGuid();
        OperationResult<ProductRole> operationResult = Operation.Fail<ProductRole>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _productRoleRepositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(productRoleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Product role not found with ID {productRoleId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenProductRoleDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid productRoleId = Guid.NewGuid();
        OperationResult<ProductRole> operationResult = Operation.Fail<ProductRole>("Failed to delete product role.", OperationErrorCode.SqlError);
        _productRoleRepositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

        ProductRoleController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(productRoleId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete product role.");
        }
    }

    private ProductRoleController CreateController(string? userName = null)
    {
        ProductRoleController controller = new(_loggerMock.Object, _productRoleRepositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}