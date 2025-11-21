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
    /// Unit tests for the <see cref="AddressTypeController"/> class.
    /// </summary>
    public class AddressTypeControllerTests
    {
        private readonly Mock<ILogger<AddressTypeController>> _loggerMock = new();
        private readonly Mock<IAddressTypeRepository> _addressTypeRepositoryMock = new();

        [Fact]
        public async Task GetById_ReturnsOk_WhenAddressTypeExists()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressTypeId = Guid.NewGuid();
            OperationResult<AddressType> operationResult = Operation.Ok(new AddressType { Id = addressTypeId });
            _addressTypeRepositoryMock.Setup(r => r.GetByIdAsync(addressTypeId, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(addressTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(operationResult.Entity);
            }
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAddressTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressTypeId = Guid.NewGuid();
            OperationResult<AddressType> operationResult = Operation.Fail<AddressType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _addressTypeRepositoryMock.Setup(r => r.GetByIdAsync(addressTypeId, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(addressTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Address type not found with ID {addressTypeId}");
            }
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenAddressTypesExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            List<AddressType> addressTypes = new() { new AddressType { Id = Guid.NewGuid() } };
            OperationResult<IEnumerable<AddressType>> operationResult = Operation.Ok<IEnumerable<AddressType>>(addressTypes);
            _addressTypeRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().BeEquivalentTo(addressTypes);
            }
        }

        [Fact]
        public async Task GetAll_ReturnsStatusCode500_WhenRepositoryFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            OperationResult<IEnumerable<AddressType>> operationResult = Operation.Fail<IEnumerable<AddressType>>("Failed to get address types.", OperationErrorCode.SqlError);
            _addressTypeRepositoryMock.Setup(r => r.GetAsync(cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Get(cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to get address types.");
            }
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenAddressTypeIsCreated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AddressType addressType = new() { Id = Guid.NewGuid() };
            OperationResult<AddressType> operationResult = Operation.Ok(addressType);
            _addressTypeRepositoryMock.Setup(r => r.AddAsync(addressType, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController("Creator UserName");

            // Act
            IActionResult result = await controller.Create(addressType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<CreatedAtActionResult>();
                CreatedAtActionResult createdResult = (CreatedAtActionResult)result;
                createdResult.Value.Should().Be(addressType);
                createdResult.RouteValues.Should().NotBeNull();
                createdResult.RouteValues!["addressTypeId"].Should().Be(addressType.Id);
                addressType.CreatedBy.Should().Be("Creator UserName");
                addressType.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
                addressType.ModifiedBy.Should().BeNull();
                addressType.Modified.Should().BeNull();
            }
        }

        [Fact]
        public async Task Create_ReturnsStatusCode500_WhenAddressTypeCreationFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AddressType addressType = new() { Id = Guid.NewGuid() };
            OperationResult<AddressType> operationResult = Operation.Fail<AddressType>("Failed to create address type.", OperationErrorCode.SqlError);
            _addressTypeRepositoryMock.Setup(r => r.AddAsync(addressType, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Create(addressType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to create address type.");
            }
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenAddressTypeIsUpdated()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AddressType addressType = new() { Id = Guid.NewGuid() };
            OperationResult<AddressType> operationResult = Operation.Ok(addressType);
            _addressTypeRepositoryMock.Setup(r => r.UpdateAsync(addressType, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController("Modifier UserName");

            // Act
            IActionResult result = await controller.Update(addressType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<OkObjectResult>();
                ((OkObjectResult)result).Value.Should().Be(addressType);
                addressType.ModifiedBy.Should().Be("Modifier UserName");
                addressType.Modified.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            }
        }

        [Fact]
        public async Task Update_ReturnsStatusCode500_WhenAddressTypeUpdateFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            AddressType addressType = new() { Id = Guid.NewGuid() };
            OperationResult<AddressType> operationResult = Operation.Fail<AddressType>("Failed to update address type.", OperationErrorCode.SqlError);
            _addressTypeRepositoryMock.Setup(r => r.UpdateAsync(addressType, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Update(addressType, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to update address type.");
            }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAddressTypeIsDeleted()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressTypeId = Guid.NewGuid();
            OperationResult<AddressType> operationResult = Operation.Ok(new AddressType { Id = addressTypeId });
            _addressTypeRepositoryMock.Setup(r => r.DeleteAsync(addressTypeId, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressTypeId, cancellationToken);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenAddressTypeDoesNotExist()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressTypeId = Guid.NewGuid();
            OperationResult<AddressType> operationResult = Operation.Fail<AddressType>(It.IsAny<string>(), OperationErrorCode.NotFound);
            _addressTypeRepositoryMock.Setup(r => r.DeleteAsync(addressTypeId, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<NotFoundObjectResult>();
                ((NotFoundObjectResult)result).Value.Should().Be($"Address type not found with ID {addressTypeId}");
            }
        }

        [Fact]
        public async Task Delete_ReturnsStatusCode500_WhenAddressTypeDeleteFails()
        {
            // Arrange
            CancellationToken cancellationToken = TestContext.Current.CancellationToken;
            Guid addressTypeId = Guid.NewGuid();
            OperationResult<AddressType> operationResult = Operation.Fail<AddressType>("Failed to delete address type.", OperationErrorCode.SqlError);
            _addressTypeRepositoryMock.Setup(r => r.DeleteAsync(addressTypeId, cancellationToken)).ReturnsAsync(operationResult);

            AddressTypeController controller = CreateController();

            // Act
            IActionResult result = await controller.Delete(addressTypeId, cancellationToken);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeOfType<ObjectResult>();
                ObjectResult objectResult = (ObjectResult)result;
                objectResult.StatusCode.Should().Be(500);
                objectResult.Value.Should().Be("Failed to delete address type.");
            }
        }

        private AddressTypeController CreateController(string? userName = null)
        {
            AddressTypeController controller = new(_loggerMock.Object, _addressTypeRepositoryMock.Object)
            {
                ControllerContext = TestHelper.CreateControllerContext(userName)
            };

            return controller;
        }
    }
}