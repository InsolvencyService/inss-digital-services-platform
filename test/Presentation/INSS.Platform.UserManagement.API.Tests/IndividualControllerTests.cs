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
/// Unit tests for the <see cref="IndividualController"/> class.
/// </summary>
public class IndividualControllerTests
{
    private readonly Mock<ILogger<IndividualController>> _loggerMock = new();
    private readonly Mock<IIndividualRepository> _individualRepositoryMock = new();

    [Fact]
    public async Task GetById_ReturnsOk_WhenIndividualExists()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid individualId = Guid.NewGuid();
        OperationResult<Individual> operationResult = Operation.Ok(new Individual { Id = individualId });
        _individualRepositoryMock.Setup(r => r.GetByIdAsync(individualId, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(individualId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
        }
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenIndividualDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid individualId = Guid.NewGuid();
        OperationResult<Individual> operationResult = Operation.Fail<Individual>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _individualRepositoryMock.Setup(r => r.GetByIdAsync(individualId, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(individualId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Individual not found with ID {individualId}");
        }
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenIndividualsExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<Individual> individuals = new() { new Individual { Id = Guid.NewGuid() } };
        OperationResult<IEnumerable<Individual>> operationResult = Operation.Ok<IEnumerable<Individual>>(individuals);
        _individualRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().BeEquivalentTo(individuals);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        OperationResult<IEnumerable<Individual>> operationResult = Operation.Fail<IEnumerable<Individual>>("Failed to get individuals.", OperationErrorCode.SqlError);
        _individualRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Get(cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to get individuals.");
        }
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenIndividualIsCreated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Individual individual = new() { Id = Guid.NewGuid() };
        OperationResult<Individual> operationResult = Operation.Ok(individual);
        _individualRepositoryMock.Setup(r => r.AddAsync(individual, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController("Creator UserName");

        // Act
        IActionResult result = await controller.Create(individual, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<CreatedAtActionResult>();
            CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
            createdResult.Value.Should().Be(individual);
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["individualId"].Should().Be(individual.Id);
            individual.CreatedBy.Should().Be("Creator UserName");
            individual.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            individual.ModifiedBy.Should().BeNull();
            individual.Modified.Should().BeNull();
        }
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenIndividualCreationFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Individual individual = new() { Id = Guid.NewGuid() };
        OperationResult<Individual> operationResult = Operation.Fail<Individual>("Failed to create individual.", OperationErrorCode.SqlError);
        _individualRepositoryMock.Setup(r => r.AddAsync(individual, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Create(individual, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to create individual.");
        }
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenIndividualIsUpdated()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Individual individual = new() { Id = Guid.NewGuid() };
        OperationResult<Individual> operationResult = Operation.Ok(individual);
        _individualRepositoryMock.Setup(r => r.UpdateAsync(individual, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController("Modifier UserName");

        // Act
        IActionResult result = await controller.Update(individual, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().Be(individual);
            individual.ModifiedBy.Should().Be("Modifier UserName");
            individual.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    [Fact]
    public async Task Update_ReturnsStatusCode500_WhenIndividualUpdateFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Individual individual = new() { Id = Guid.NewGuid() };
        OperationResult<Individual> operationResult = Operation.Fail<Individual>("Failed to update individual.", OperationErrorCode.SqlError);
        _individualRepositoryMock.Setup(r => r.UpdateAsync(individual, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Update(individual, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to update individual.");
        }
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenIndividualIsDeleted()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid individualId = Guid.NewGuid();
        OperationResult<Individual> operationResult = Operation.Ok(new Individual { Id = individualId });
        _individualRepositoryMock.Setup(r => r.DeleteAsync(individualId, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(individualId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenIndividualDoesNotExist()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid individualId = Guid.NewGuid();
        OperationResult<Individual> operationResult = Operation.Fail<Individual>(It.IsAny<string>(), OperationErrorCode.NotFound);
        _individualRepositoryMock.Setup(r => r.DeleteAsync(individualId, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(individualId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be($"Individual not found with ID {individualId}");
        }
    }

    [Fact]
    public async Task Delete_ReturnsStatusCode500_WhenIndividualDeleteFails()
    {
        // Arrange
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Guid individualId = Guid.NewGuid();
        OperationResult<Individual> operationResult = Operation.Fail<Individual>("Failed to delete individual.", OperationErrorCode.SqlError);
        _individualRepositoryMock.Setup(r => r.DeleteAsync(individualId, cancellationToken)).ReturnsAsync(operationResult);

        IndividualController controller = CreateController();

        // Act
        IActionResult result = await controller.Delete(individualId, cancellationToken);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeOfType<ObjectResult>();
            ObjectResult objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("Failed to delete individual.");
        }
    }

    private IndividualController CreateController(string? userName = null)
    {
        IndividualController controller = new(_loggerMock.Object, _individualRepositoryMock.Object)
        {
            ControllerContext = TestHelper.CreateControllerContext(userName)
        };

        return controller;
    }
}