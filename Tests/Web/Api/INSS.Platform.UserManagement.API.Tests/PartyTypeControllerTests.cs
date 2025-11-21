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
    /// Unit tests for the <see cref="PartyTypeController"/> class.
    /// </summary>
    public class PartyTypeControllerTests
    {
        private readonly Mock<ILogger<PartyTypeController>> _loggerMock = new();
        private readonly Mock<IPartyTypeRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenPartyTypeExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyTypeId = Guid.NewGuid();
            OperationResult<PartyType> operationResult = Operation.Ok(new PartyType { Id = partyTypeId });
            _repositoryMock.Setup(r => r.GetByIdAsync(partyTypeId, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(partyTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPartyTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyTypeId = Guid.NewGuid();
            OperationResult<PartyType> operationResult = Operation.Fail<PartyType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(partyTypeId, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(partyTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party type not found with ID {partyTypeId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenPartyTypesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<PartyType> partyTypes = new() { new PartyType { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<PartyType>> operationResult = Operation.Ok<IEnumerable<PartyType>>(partyTypes);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(partyTypes);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<PartyType>> operationResult = Operation.Fail<IEnumerable<PartyType>>("Failed to get party types.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get party types.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenPartyTypeIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyType partyType = new() { Id = Guid.NewGuid() };
            OperationResult<PartyType> operationResult = Operation.Ok(partyType);
            _repositoryMock.Setup(r => r.AddAsync(partyType, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(partyType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(partyType);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["partyTypeId"].Should().Be(partyType.Id);
                partyType.CreatedBy.Should().Be("Creator UserName");
                partyType.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                partyType.ModifiedBy.Should().BeNull();
                partyType.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenPartyTypeCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyType partyType = new() { Id = Guid.NewGuid() };
            OperationResult<PartyType> operationResult = Operation.Fail<PartyType>("Failed to create party type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(partyType, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(partyType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create party type.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenPartyTypeIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyType partyType = new() { Id = Guid.NewGuid() };
            OperationResult<PartyType> operationResult = Operation.Ok(partyType);
            _repositoryMock.Setup(r => r.UpdateAsync(partyType, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(partyType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(partyType);
                partyType.ModifiedBy.Should().Be("Modifier UserName");
                partyType.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenPartyTypeUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyType partyType = new() { Id = Guid.NewGuid() };
            OperationResult<PartyType> operationResult = Operation.Fail<PartyType>("Failed to update party type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(partyType, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(partyType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update party type.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenPartyTypeIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyTypeId = Guid.NewGuid();
            OperationResult<PartyType> operationResult = Operation.Ok(new PartyType { Id = partyTypeId });
            _repositoryMock.Setup(r => r.DeleteAsync(partyTypeId, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyTypeId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPartyTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyTypeId = Guid.NewGuid();
            OperationResult<PartyType> operationResult = Operation.Fail<PartyType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(partyTypeId, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party type not found with ID {partyTypeId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenPartyTypeDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyTypeId = Guid.NewGuid();
            OperationResult<PartyType> operationResult = Operation.Fail<PartyType>("Failed to delete party type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(partyTypeId, cancellationToken)).ReturnsAsync(operationResult);

            PartyTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete party type.");
            }
        }

        private PartyTypeController CreateController(string? userName = null)
        {
            PartyTypeController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}