using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Events.Domain.Tests;

public class DomainEventTests
{
    [Fact]
    public void DomainEvent_WithActor_SetsActorProperty()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        TestDomainEvent domainEvent = new(actor, aggregateRootId, correlationId);

        // Assert
        domainEvent.Actor.Should().Be(actor);
    }

    [Fact]
    public void DomainEvent_WithAggregateRootId_SetsAggregateRootIdProperty()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        TestDomainEvent domainEvent = new(actor, aggregateRootId, correlationId);

        // Assert
        domainEvent.AggregateRootId.Should().Be(aggregateRootId);
    }

    [Fact]
    public void DomainEvent_WithCorrelationId_SetsCorrelationIdProperty()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        TestDomainEvent domainEvent = new(actor, aggregateRootId, correlationId);

        // Assert
        domainEvent.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void DomainEvent_GeneratesUniqueEventId()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent event1 = new(actor);
        TestDomainEvent event2 = new(actor);

        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }

    [Fact]
    public void DomainEvent_WithoutAggregateRootId_GeneratesNewGuid()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor);

        // Assert
        domainEvent.AggregateRootId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DomainEvent_WithoutCorrelationId_GeneratesNewGuid()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor);

        // Assert
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DomainEvent_WithNullAggregateRootId_GeneratesNewGuid()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor, null, null);

        // Assert
        domainEvent.AggregateRootId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DomainEvent_WithNullCorrelationId_GeneratesNewGuid()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor, null, null);

        // Assert
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DomainEvent_ImplementsIDomainEvent()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor);

        // Assert
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void DomainEvent_IsRecord()
    {
        // Arrange & Act
        TestDomainEvent event1 = new("Actor", Guid.NewGuid(), Guid.NewGuid());
        TestDomainEvent event2 = new("Actor", event1.AggregateRootId, event1.CorrelationId);

        // Assert - Records with same values should be equal (value equality)
        event1.Actor.Should().Be(event2.Actor);
        event1.AggregateRootId.Should().Be(event2.AggregateRootId);
        event1.CorrelationId.Should().Be(event2.CorrelationId);
    }

    [Fact]
    public void DomainEvent_EventId_IsNotEmpty()
    {
        // Arrange
        string actor = "TestUser";

        // Act
        TestDomainEvent domainEvent = new(actor);

        // Assert
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DomainEvent_WithAllParameters_SetsAllProperties()
    {
        // Arrange
        string actor = "SystemUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        TestDomainEvent domainEvent = new(actor, aggregateRootId, correlationId);

        // Assert
        using (new AssertionScope())
        {
            domainEvent.Actor.Should().Be(actor);
            domainEvent.AggregateRootId.Should().Be(aggregateRootId);
            domainEvent.CorrelationId.Should().Be(correlationId);
            domainEvent.EventId.Should().NotBe(Guid.Empty);
            domainEvent.OccurredOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }

    // Test helper class
    private sealed record TestDomainEvent : DomainEvent
    {
        public TestDomainEvent(string actor, Guid? aggregateRootId = null, Guid? correlationId = null)
            : base(actor, aggregateRootId, correlationId)
        {
        }
    }
}