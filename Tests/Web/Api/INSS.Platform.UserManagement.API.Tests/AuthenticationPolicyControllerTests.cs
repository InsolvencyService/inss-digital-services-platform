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
    /// Unit tests for the <see cref="AuthenticationPolicyController"/> class.
    /// </summary>
    public class AuthenticationPolicyControllerTests
    {
        private readonly Mock<ILogger<AuthenticationPolicyController>> _loggerMock = new();
        private readonly Mock<IAuthenticationPolicyRepository> _authenticationPolicyRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenAuthenticationPolicyExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid policyId = Guid.NewGuid();
            OperationResult<AuthenticationPolicy> operationResult = Operation.Ok(new AuthenticationPolicy { Id = policyId });
            _authenticationPolicyRepositoryMock.Setup(r => r.GetByIdAsync(policyId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(policyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAuthenticationPolicyDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid policyId = Guid.NewGuid();
            OperationResult<AuthenticationPolicy> operationResult = Operation.Fail<AuthenticationPolicy>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _authenticationPolicyRepositoryMock.Setup(r => r.GetByIdAsync(policyId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(policyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication policy not found with ID {policyId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenAuthenticationPoliciesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<AuthenticationPolicy> policies = new() { new AuthenticationPolicy { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<AuthenticationPolicy>> operationResult = Operation.Ok<IEnumerable<AuthenticationPolicy>>(policies);
            _authenticationPolicyRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(policies);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<AuthenticationPolicy>> operationResult = Operation.Fail<IEnumerable<AuthenticationPolicy>>("Failed to get authentication policies.", OperationErrorCode.SqlError);
            _authenticationPolicyRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get authentication policies.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenAuthenticationPolicyIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationPolicy policy = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationPolicy> operationResult = Operation.Ok(policy);
            _authenticationPolicyRepositoryMock.Setup(r => r.AddAsync(policy, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(policy, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(policy);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["authenticationPolicyId"].Should().Be(policy.Id);
                policy.CreatedBy.Should().Be("Creator UserName");
                policy.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                policy.ModifiedBy.Should().BeNull();
                policy.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenAuthenticationPolicyCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationPolicy policy = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationPolicy> operationResult = Operation.Fail<AuthenticationPolicy>("Failed to create authentication policy.", OperationErrorCode.SqlError);
            _authenticationPolicyRepositoryMock.Setup(r => r.AddAsync(policy, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(policy, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create authentication policy.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenAuthenticationPolicyIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationPolicy policy = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationPolicy> operationResult = Operation.Ok(policy);
            _authenticationPolicyRepositoryMock.Setup(r => r.UpdateAsync(policy, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(policy, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(policy);
                policy.ModifiedBy.Should().Be("Modifier UserName");
                policy.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenAuthenticationPolicyUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AuthenticationPolicy policy = new() { Id = Guid.NewGuid() };
            OperationResult<AuthenticationPolicy> operationResult = Operation.Fail<AuthenticationPolicy>("Failed to update authentication policy.", OperationErrorCode.SqlError);
            _authenticationPolicyRepositoryMock.Setup(r => r.UpdateAsync(policy, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(policy, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update authentication policy.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAuthenticationPolicyIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid policyId = Guid.NewGuid();
            OperationResult<AuthenticationPolicy> operationResult = Operation.Ok(new AuthenticationPolicy { Id = policyId });
            _authenticationPolicyRepositoryMock.Setup(r => r.DeleteAsync(policyId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(policyId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAuthenticationPolicyDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid policyId = Guid.NewGuid();
            OperationResult<AuthenticationPolicy> operationResult = Operation.Fail<AuthenticationPolicy>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _authenticationPolicyRepositoryMock.Setup(r => r.DeleteAsync(policyId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(policyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Authentication policy not found with ID {policyId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenAuthenticationPolicyDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid policyId = Guid.NewGuid();
            OperationResult<AuthenticationPolicy> operationResult = Operation.Fail<AuthenticationPolicy>("Failed to delete authentication policy.", OperationErrorCode.SqlError);
            _authenticationPolicyRepositoryMock.Setup(r => r.DeleteAsync(policyId, cancellationToken)).ReturnsAsync(operationResult);

            AuthenticationPolicyController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(policyId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete authentication policy.");
            }
        }

        private AuthenticationPolicyController CreateController(string? userName = null)
        {
            AuthenticationPolicyController controller = new(_loggerMock.Object, _authenticationPolicyRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}