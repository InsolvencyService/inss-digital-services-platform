using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Events.Domain;
using Moq;

namespace INSS.Platform.Audit.Application.Tests;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_SingleEvent_CallsSingleHandler()
    {
        // Arrange
        UserBankDetailsAddedEvent domainEvent = new("System", Guid.NewGuid(), Guid.NewGuid(), "test account", "12-34-56");

        Mock<IDomainEventHandler<UserBankDetailsAddedEvent>> handlerMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IDomainEventHandler<UserBankDetailsAddedEvent>>)))
            .Returns(new[] { handlerMock.Object });

        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act
        await dispatcher.DispatchAsync([domainEvent], CancellationToken.None);

        // Assert
        handlerMock.Verify(h => h.Handle(domainEvent, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_MultipleEvents_CallsAllHandlers()
    {
        // Arrange
        UserBankDetailsAddedEvent event1 = new("System", Guid.NewGuid(), Guid.NewGuid(), "account 1", "12-34-56");
        UserBankDetailsAddedEvent event2 = new("System", Guid.NewGuid(), Guid.NewGuid(), "account 2", "78-90-12");

        Mock<IDomainEventHandler<UserBankDetailsAddedEvent>> handlerMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IDomainEventHandler<UserBankDetailsAddedEvent>>)))
            .Returns(new[] { handlerMock.Object });

        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act
        await dispatcher.DispatchAsync([event1, event2], CancellationToken.None);

        // Assert
        handlerMock.Verify(h => h.Handle(event1, CancellationToken.None), Times.Once);
        handlerMock.Verify(h => h.Handle(event2, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_SingleEventMultipleHandlers_CallsAllHandlers()
    {
        // Arrange
        UserBankDetailsAddedEvent domainEvent = new("System", Guid.NewGuid(), Guid.NewGuid(), "test account", "12-34-56");

        Mock<IDomainEventHandler<UserBankDetailsAddedEvent>> handler1Mock = new();
        Mock<IDomainEventHandler<UserBankDetailsAddedEvent>> handler2Mock = new();
        Mock<IServiceProvider> serviceProviderMock = new();

        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IDomainEventHandler<UserBankDetailsAddedEvent>>)))
            .Returns(new[] { handler1Mock.Object, handler2Mock.Object });

        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act
        await dispatcher.DispatchAsync([domainEvent], CancellationToken.None);

        // Assert
        handler1Mock.Verify(h => h.Handle(domainEvent, CancellationToken.None), Times.Once);
        handler2Mock.Verify(h => h.Handle(domainEvent, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_NoHandlersRegistered_CompletesWithoutError()
    {
        // Arrange
        UserBankDetailsAddedEvent domainEvent = new("System", Guid.NewGuid(), Guid.NewGuid(), "test account", "12-34-56");

        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IDomainEventHandler<UserBankDetailsAddedEvent>>)))
            .Returns(Array.Empty<IDomainEventHandler<UserBankDetailsAddedEvent>>());

        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act & Assert - Should not throw
        await dispatcher.DispatchAsync([domainEvent], CancellationToken.None);
    }

    [Fact]
    public async Task DispatchAsync_EmptyEventList_CompletesWithoutError()
    {
        // Arrange
        Mock<IServiceProvider> serviceProviderMock = new();
        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act & Assert - Should not throw
        await dispatcher.DispatchAsync([], CancellationToken.None);
    }

    [Fact]
    public async Task DispatchAsync_HandlerThrowsException_PropagatesException()
    {
        // Arrange
        UserBankDetailsAddedEvent domainEvent = new("System", Guid.NewGuid(), Guid.NewGuid(), "test account", "12-34-56");

        Mock<IDomainEventHandler<UserBankDetailsAddedEvent>> handlerMock = new();
        handlerMock
            .Setup(h => h.Handle(It.IsAny<UserBankDetailsAddedEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Handler error"));

        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IEnumerable<IDomainEventHandler<UserBankDetailsAddedEvent>>)))
            .Returns(new[] { handlerMock.Object });

        DomainEventDispatcher dispatcher = new(serviceProviderMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await dispatcher.DispatchAsync([domainEvent], CancellationToken.None));
    }
}
