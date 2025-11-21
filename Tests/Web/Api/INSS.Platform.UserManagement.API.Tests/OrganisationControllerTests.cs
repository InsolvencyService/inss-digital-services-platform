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
    /// Unit tests for the <see cref="OrganisationController"/> class.
    /// </summary>
    public class OrganisationControllerTests
    {
        private readonly Mock<ILogger<OrganisationController>> _loggerMock = new();
        private readonly Mock<IOrganisationRepository> _organisationRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenOrganisationExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid organisationId = Guid.NewGuid();
            OperationResult<Organisation> operationResult = Operation.Ok(new Organisation { Id = organisationId });
            _organisationRepositoryMock.Setup(r => r.GetByIdAsync(organisationId, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(organisationId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid organisationId = Guid.NewGuid();
            OperationResult<Organisation> operationResult = Operation.Fail<Organisation>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _organisationRepositoryMock.Setup(r => r.GetByIdAsync(organisationId, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(organisationId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Organisation not found with ID {organisationId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenOrganisationsExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<Organisation> organisations = new() { new Organisation { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<Organisation>> operationResult = Operation.Ok<IEnumerable<Organisation>>(organisations);
            _organisationRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(organisations);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<Organisation>> operationResult = Operation.Fail<IEnumerable<Organisation>>("Failed to get organisations.", OperationErrorCode.SqlError);
            _organisationRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get organisations.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenOrganisationIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Organisation organisation = new() { Id = Guid.NewGuid() };
            OperationResult<Organisation> operationResult = Operation.Ok(organisation);
            _organisationRepositoryMock.Setup(r => r.AddAsync(organisation, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(organisation, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(organisation);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["organisationId"].Should().Be(organisation.Id);
                organisation.CreatedBy.Should().Be("Creator UserName");
                organisation.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                organisation.ModifiedBy.Should().BeNull();
                organisation.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenOrganisationCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Organisation organisation = new() { Id = Guid.NewGuid() };
            OperationResult<Organisation> operationResult = Operation.Fail<Organisation>("Failed to create organisation.", OperationErrorCode.SqlError);
            _organisationRepositoryMock.Setup(r => r.AddAsync(organisation, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(organisation, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create organisation.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenOrganisationIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Organisation organisation = new() { Id = Guid.NewGuid() };
            OperationResult<Organisation> operationResult = Operation.Ok(organisation);
            _organisationRepositoryMock.Setup(r => r.UpdateAsync(organisation, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(organisation, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(organisation);
                organisation.ModifiedBy.Should().Be("Modifier UserName");
                organisation.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenOrganisationUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Organisation organisation = new() { Id = Guid.NewGuid() };
            OperationResult<Organisation> operationResult = Operation.Fail<Organisation>("Failed to update organisation.", OperationErrorCode.SqlError);
            _organisationRepositoryMock.Setup(r => r.UpdateAsync(organisation, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(organisation, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update organisation.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenOrganisationIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid organisationId = Guid.NewGuid();
            OperationResult<Organisation> operationResult = Operation.Ok(new Organisation { Id = organisationId });
            _organisationRepositoryMock.Setup(r => r.DeleteAsync(organisationId, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(organisationId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenOrganisationDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid organisationId = Guid.NewGuid();
            OperationResult<Organisation> operationResult = Operation.Fail<Organisation>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _organisationRepositoryMock.Setup(r => r.DeleteAsync(organisationId, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(organisationId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Organisation not found with ID {organisationId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenOrganisationDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid organisationId = Guid.NewGuid();
            OperationResult<Organisation> operationResult = Operation.Fail<Organisation>("Failed to delete organisation.", OperationErrorCode.SqlError);
            _organisationRepositoryMock.Setup(r => r.DeleteAsync(organisationId, cancellationToken)).ReturnsAsync(operationResult);

            OrganisationController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(organisationId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete organisation.");
            }
        }

        private OrganisationController CreateController(string? userName = null)
        {
            OrganisationController controller = new(_loggerMock.Object, _organisationRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}