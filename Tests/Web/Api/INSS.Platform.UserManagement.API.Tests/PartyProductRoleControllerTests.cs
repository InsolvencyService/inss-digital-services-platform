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
    /// Unit tests for the <see cref="PartyProductRoleController"/> class.
    /// </summary>
    public class PartyProductRoleControllerTests
    {
        private readonly Mock<ILogger<PartyProductRoleController>> _loggerMock = new();
        private readonly Mock<IPartyProductRoleRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductRoleExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productRoleId = Guid.NewGuid();
            OperationResult<PartyProductRole> operationResult = Operation.Ok(new PartyProductRole { Id = productRoleId });
            _repositoryMock.Setup(r => r.GetByIdAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

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
            OperationResult<PartyProductRole> operationResult = Operation.Fail<PartyProductRole>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(productRoleId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party product role not found with ID {productRoleId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenProductRolesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<PartyProductRole> productRoles = new() { new PartyProductRole { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<PartyProductRole>> operationResult = Operation.Ok<IEnumerable<PartyProductRole>>(productRoles);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

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
            OperationResult<IEnumerable<PartyProductRole>> operationResult = Operation.Fail<IEnumerable<PartyProductRole>>("Failed to get party product roles.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get party product roles.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenProductRoleIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyProductRole productRole = new() { Id = Guid.NewGuid() };
            OperationResult<PartyProductRole> operationResult = Operation.Ok(productRole);
            _repositoryMock.Setup(r => r.AddAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(productRole, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(productRole);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["partyProductRoleId"].Should().Be(productRole.Id);
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
            PartyProductRole productRole = new() { Id = Guid.NewGuid() };
            OperationResult<PartyProductRole> operationResult = Operation.Fail<PartyProductRole>("Failed to create party product role.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(productRole, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create party product role.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenProductRoleIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            PartyProductRole productRole = new() { Id = Guid.NewGuid() };
            OperationResult<PartyProductRole> operationResult = Operation.Ok(productRole);
            _repositoryMock.Setup(r => r.UpdateAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController("Modifier UserName");

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
            PartyProductRole productRole = new() { Id = Guid.NewGuid() };
            OperationResult<PartyProductRole> operationResult = Operation.Fail<PartyProductRole>("Failed to update party product role.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(productRole, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(productRole, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update party product role.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProductRoleIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productRoleId = Guid.NewGuid();
            OperationResult<PartyProductRole> operationResult = Operation.Ok(new PartyProductRole { Id = productRoleId });
            _repositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

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
            OperationResult<PartyProductRole> operationResult = Operation.Fail<PartyProductRole>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(productRoleId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Party product role not found with ID {productRoleId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenProductRoleDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid productRoleId = Guid.NewGuid();
            OperationResult<PartyProductRole> operationResult = Operation.Fail<PartyProductRole>("Failed to delete party product role.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(productRoleId, cancellationToken)).ReturnsAsync(operationResult);

            PartyProductRoleController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(productRoleId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete party product role.");
            }
        }

        private PartyProductRoleController CreateController(string? userName = null)
        {
            PartyProductRoleController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}