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
    /// Unit tests for the <see cref="PartyController"/> class.
    /// </summary>
    public class PartyControllerTests
    {
        private readonly Mock<ILogger<PartyController>> _loggerMock = new();
        private readonly Mock<IPartyRepository> _partyRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenPartyExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyId = Guid.NewGuid();
            OperationResult<Party> operationResult = Operation.Ok(new Party { Id = partyId });
            _partyRepositoryMock.Setup(r => r.GetByIdAsync(partyId, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(partyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPartyDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyId = Guid.NewGuid();
            OperationResult<Party> operationResult = Operation.Fail<Party>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _partyRepositoryMock.Setup(r => r.GetByIdAsync(partyId, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(partyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party not found with ID {partyId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenPartiesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<Party> parties = new() { new Party { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<Party>> operationResult = Operation.Ok<IEnumerable<Party>>(parties);
            _partyRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(parties);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<Party>> operationResult = Operation.Fail<IEnumerable<Party>>("Failed to get parties.", OperationErrorCode.SqlError);
            _partyRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get parties.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenPartyIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Party party = new() { Id = Guid.NewGuid() };
            OperationResult<Party> operationResult = Operation.Ok(party);
            _partyRepositoryMock.Setup(r => r.AddAsync(party, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(party, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(party);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["partyId"].Should().Be(party.Id);
                party.CreatedBy.Should().Be("Creator UserName");
                party.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                party.ModifiedBy.Should().BeNull();
                party.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenPartyCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Party party = new() { Id = Guid.NewGuid() };
            OperationResult<Party> operationResult = Operation.Fail<Party>("Failed to create party.", OperationErrorCode.SqlError);
            _partyRepositoryMock.Setup(r => r.AddAsync(party, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(party, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create party.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenPartyIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Party party = new() { Id = Guid.NewGuid() };
            OperationResult<Party> operationResult = Operation.Ok(party);
            _partyRepositoryMock.Setup(r => r.UpdateAsync(party, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(party, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(party);
                party.ModifiedBy.Should().Be("Modifier UserName");
                party.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenPartyUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Party party = new() { Id = Guid.NewGuid() };
            OperationResult<Party> operationResult = Operation.Fail<Party>("Failed to update party.", OperationErrorCode.SqlError);
            _partyRepositoryMock.Setup(r => r.UpdateAsync(party, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(party, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update party.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenPartyIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyId = Guid.NewGuid();
            OperationResult<Party> operationResult = Operation.Ok(new Party { Id = partyId });
            _partyRepositoryMock.Setup(r => r.DeleteAsync(partyId, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPartyDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyId = Guid.NewGuid();
            OperationResult<Party> operationResult = Operation.Fail<Party>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _partyRepositoryMock.Setup(r => r.DeleteAsync(partyId, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party not found with ID {partyId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenPartyDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid partyId = Guid.NewGuid();
            OperationResult<Party> operationResult = Operation.Fail<Party>("Failed to delete party.", OperationErrorCode.SqlError);
            _partyRepositoryMock.Setup(r => r.DeleteAsync(partyId, cancellationToken)).ReturnsAsync(operationResult);

            PartyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(partyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete party.");
            }
        }

        private PartyController CreateController(string? userName = null)
        {
            PartyController controller = new(_loggerMock.Object, _partyRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}