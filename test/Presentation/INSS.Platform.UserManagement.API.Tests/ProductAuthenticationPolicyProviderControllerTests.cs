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
/// Unit tests for the <see cref="ProductAuthenticationPolicyProviderController"/> class.
/// </summary>
public class ProductAuthenticationPolicyProviderControllerTests
{
    private readonly Mock<ILogger<ProductAuthenticationPolicyProviderController>> _loggerMock = new();
    private readonly Mock<IProductAuthenticationPolicyProviderRepository> _repositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenProviderExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid providerId = Guid.NewGuid();
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Ok(new ProductAuthenticationPolicyProvider { Id = providerId });
        _repositoryMock.Setup(r => r.GetByIdAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(providerId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProviderDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid providerId = Guid.NewGuid();
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Fail<ProductAuthenticationPolicyProvider>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.GetByIdAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(providerId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Product authentication policy provider not found with ID {providerId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenProvidersExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<ProductAuthenticationPolicyProvider> providers = new() { new ProductAuthenticationPolicyProvider { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<ProductAuthenticationPolicyProvider>> operationResult = Operation.Ok<IEnumerable<ProductAuthenticationPolicyProvider>>(providers);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(providers);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<ProductAuthenticationPolicyProvider>> operationResult = Operation.Fail<IEnumerable<ProductAuthenticationPolicyProvider>>("Failed to get product authentication policy providers.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get product authentication policy providers.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenProviderIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductAuthenticationPolicyProvider provider = new() { Id = Guid.NewGuid() };
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Ok(provider);
        _repositoryMock.Setup(r => r.AddAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(provider, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(provider);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["productAuthenticationPolicyProviderId"].Should().Be(provider.Id);
            provider.CreatedBy.Should().Be("Creator UserName");
            provider.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            provider.ModifiedBy.Should().BeNull();
            provider.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenProviderCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductAuthenticationPolicyProvider provider = new() { Id = Guid.NewGuid() };
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Fail<ProductAuthenticationPolicyProvider>("Failed to create product authentication policy provider.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.AddAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(provider, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create product authentication policy provider.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenProviderIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductAuthenticationPolicyProvider provider = new() { Id = Guid.NewGuid() };
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Ok(provider);
        _repositoryMock.Setup(r => r.UpdateAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(provider, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(provider);
            provider.ModifiedBy.Should().Be("Modifier UserName");
            provider.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenProviderUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        ProductAuthenticationPolicyProvider provider = new() { Id = Guid.NewGuid() };
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Fail<ProductAuthenticationPolicyProvider>("Failed to update product authentication policy provider.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.UpdateAsync(provider, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(provider, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update product authentication policy provider.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProviderIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid providerId = Guid.NewGuid();
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Ok(new ProductAuthenticationPolicyProvider { Id = providerId });
        _repositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(providerId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProviderDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid providerId = Guid.NewGuid();
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Fail<ProductAuthenticationPolicyProvider>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _repositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(providerId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Product authentication policy provider not found with ID {providerId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenProviderDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid providerId = Guid.NewGuid();
        OperationResult<ProductAuthenticationPolicyProvider> operationResult = Operation.Fail<ProductAuthenticationPolicyProvider>("Failed to delete product authentication policy provider.", OperationErrorCode.SqlError);
        _repositoryMock.Setup(r => r.DeleteAsync(providerId, cancellationToken)).ReturnsAsync(operationResult);

        ProductAuthenticationPolicyProviderController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(providerId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete product authentication policy provider.");
        }
    }

    private ProductAuthenticationPolicyProviderController CreateController(string? userName = null)
    {
        ProductAuthenticationPolicyProviderController controller = new(_loggerMock.Object, _repositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}