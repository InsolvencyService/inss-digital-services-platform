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
    /// Unit tests for the <see cref="RelationshipTypeController"/> class.
    /// </summary>
    public class RelationshipTypeControllerTests
    {
        private readonly Mock<ILogger<RelationshipTypeController>> _loggerMock = new();
        private readonly Mock<IRelationshipTypeRepository> _repositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenRelationshipTypeExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<RelationshipType> operationResult = Operation.Ok(new RelationshipType { Id = id });
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<RelationshipType> operationResult = Operation.Fail<RelationshipType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.GetByIdAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Relationship type not found with ID {id}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenRelationshipTypesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<RelationshipType> entities = new() { new RelationshipType { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<RelationshipType>> operationResult = Operation.Ok<IEnumerable<RelationshipType>>(entities);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(entities);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<RelationshipType>> operationResult = Operation.Fail<IEnumerable<RelationshipType>>("Failed to get relationship types.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get relationship types.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenRelationshipTypeIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            RelationshipType entity = new() { Id = Guid.NewGuid() };
            OperationResult<RelationshipType> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(entity);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["relationshipTypeId"].Should().Be(entity.Id);
                entity.CreatedBy.Should().Be("Creator UserName");
                entity.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                entity.ModifiedBy.Should().BeNull();
                entity.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            RelationshipType entity = new() { Id = Guid.NewGuid() };
            OperationResult<RelationshipType> operationResult = Operation.Fail<RelationshipType>("Failed to create relationship type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.AddAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create relationship type.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenRelationshipTypeIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            RelationshipType entity = new() { Id = Guid.NewGuid() };
            OperationResult<RelationshipType> operationResult = Operation.Ok(entity);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(entity);
                entity.ModifiedBy.Should().Be("Modifier UserName");
                entity.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            RelationshipType entity = new() { Id = Guid.NewGuid() };
            OperationResult<RelationshipType> operationResult = Operation.Fail<RelationshipType>("Failed to update relationship type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.UpdateAsync(entity, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(entity, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update relationship type.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenRelationshipTypeIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<RelationshipType> operationResult = Operation.Ok(new RelationshipType { Id = id });
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenRelationshipTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<RelationshipType> operationResult = Operation.Fail<RelationshipType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Relationship type not found with ID {id}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid id = Guid.NewGuid();
            OperationResult<RelationshipType> operationResult = Operation.Fail<RelationshipType>("Failed to delete relationship type.", OperationErrorCode.SqlError);
            _repositoryMock.Setup(r => r.DeleteAsync(id, cancellationToken)).ReturnsAsync(operationResult);

            RelationshipTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(id, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete relationship type.");
            }
        }

        private RelationshipTypeController CreateController(string? userName = null)
        {
            RelationshipTypeController controller = new(_loggerMock.Object, _repositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}