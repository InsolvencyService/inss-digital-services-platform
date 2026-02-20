using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Events.Domain;
using Moq;

namespace INSS.Platform.Canonical.Domain.Tests;

public class BaseEntityTests
{
    [Fact]
    public void BaseEntity_CanBeInstantiated()
    {
        // Arrange & Act
        TestEntity entity = new ();

        // Assert
        using (new AssertionScope())
        {
            entity.Should().NotBeNull();
            entity.Id.Should().Be(Guid.Empty);
            entity.InstanceId.Should().Be(Guid.Empty);
            entity.Created.Should().BeNull();
            entity.CreatedBy.Should().Be(string.Empty);
            entity.Modified.Should().BeNull();
            entity.ModifiedBy.Should().BeNull();
            entity.DomainEvents.Should().BeEmpty();
        }
    }

    [Fact]
    public void BaseEntity_Id_CanBeSet()
    {
        // Arrange
        TestEntity entity = new ();
        Guid expectedId = Guid.NewGuid();

        // Act
        entity.Id = expectedId;

        // Assert
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void BaseEntity_InstanceId_CanBeSet()
    {
        // Arrange
        TestEntity entity = new();
        Guid expectedInstanceId = Guid.NewGuid();

        // Act
        entity.InstanceId = expectedInstanceId;

        // Assert
        entity.InstanceId.Should().Be(expectedInstanceId);
    }

    [Fact]
    public void BaseEntity_Created_CanBeSet()
    {
        // Arrange
        TestEntity entity = new();
        DateTime expectedCreated = DateTime.UtcNow;

        // Act
        entity.Created = expectedCreated;

        // Assert
        entity.Created.Should().Be(expectedCreated);
    }

    [Fact]
    public void BaseEntity_CreatedBy_CanBeSet()
    {
        // Arrange
        TestEntity entity = new();
        string expectedCreatedBy = "TestUser";

        // Act
        entity.CreatedBy = expectedCreatedBy;

        // Assert
        entity.CreatedBy.Should().Be(expectedCreatedBy);
    }

    [Fact]
    public void BaseEntity_Modified_CanBeSet()
    {
        // Arrange
        TestEntity entity = new();
        DateTime expectedModified = DateTime.UtcNow;

        // Act
        entity.Modified = expectedModified;

        // Assert
        entity.Modified.Should().Be(expectedModified);
    }

    [Fact]
    public void BaseEntity_ModifiedBy_CanBeSet()
    {
        // Arrange
        TestEntity entity = new();
        string expectedModifiedBy = "TestUser";

        // Act
        entity.ModifiedBy = expectedModifiedBy;

        // Assert
        entity.ModifiedBy.Should().Be(expectedModifiedBy);
    }

    [Fact]
    public void BaseEntity_DomainEvents_InitiallyEmpty()
    {
        // Arrange & Act
        TestEntity entity = new();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void AddDomainEvent_AddsSingleEvent()
    {
        // Arrange
        TestEntity entity = new();
        Mock<IDomainEvent> domainEventMock = new ();

        // Act
        entity.AddTestDomainEvent(domainEventMock.Object);

        // Assert
        using (new AssertionScope())
        {
            entity.DomainEvents.Should().HaveCount(1);
            entity.DomainEvents.Should().Contain(domainEventMock.Object);
        }
    }

    [Fact]
    public void AddDomainEvent_AddsMultipleEvents()
    {
        // Arrange
        TestEntity entity = new();
        Mock<IDomainEvent> event1Mock = new();
        Mock<IDomainEvent> event2Mock = new();
        Mock<IDomainEvent> event3Mock = new();

        // Act
        entity.AddTestDomainEvent(event1Mock.Object);
        entity.AddTestDomainEvent(event2Mock.Object);
        entity.AddTestDomainEvent(event3Mock.Object);

        // Assert
        using (new AssertionScope())
        {
            entity.DomainEvents.Should().HaveCount(3);
            entity.DomainEvents.Should().Contain(event1Mock.Object);
            entity.DomainEvents.Should().Contain(event2Mock.Object);
            entity.DomainEvents.Should().Contain(event3Mock.Object);
        }
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        TestEntity entity = new();
        Mock<IDomainEvent> event1Mock = new();
        Mock<IDomainEvent> event2Mock = new();
        entity.AddTestDomainEvent(event1Mock.Object);
        entity.AddTestDomainEvent(event2Mock.Object);

        // Act
        entity.ClearDomainEvents();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_WhenNoEvents_DoesNotThrow()
    {
        // Arrange
        TestEntity entity = new();

        // Act
        Action act = entity.ClearDomainEvents;

        // Assert
        act.Should().NotThrow();
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void DomainEvents_IsReadOnly()
    {
        // Arrange
        TestEntity entity = new();

        // Act
        IReadOnlyList<IDomainEvent> events = entity.DomainEvents;

        // Assert
        events.Should().BeAssignableTo<IReadOnlyList<IDomainEvent>>();
    }

    [Fact]
    public void BaseEntity_WithAllPropertiesSet_RetainsValues()
    {
        // Arrange
        TestEntity entity = new();
        Guid id = Guid.NewGuid();
        Guid instanceId = Guid.NewGuid();
        DateTime created = DateTime.UtcNow.AddDays(-5);
        string createdBy = "Creator";
        DateTime modified = DateTime.UtcNow;
        string modifiedBy = "Modifier";

        // Act
        entity.Id = id;
        entity.InstanceId = instanceId;
        entity.Created = created;
        entity.CreatedBy = createdBy;
        entity.Modified = modified;
        entity.ModifiedBy = modifiedBy;

        // Assert
        using (new AssertionScope())
        {
            entity.Id.Should().Be(id);
            entity.InstanceId.Should().Be(instanceId);
            entity.Created.Should().Be(created);
            entity.CreatedBy.Should().Be(createdBy);
            entity.Modified.Should().Be(modified);
            entity.ModifiedBy.Should().Be(modifiedBy);
        }
    }

    [Fact]
    public void AddDomainEvent_PreservesEventOrder()
    {
        // Arrange
        TestEntity entity = new();
        Mock<IDomainEvent> event1Mock = new();
        Mock<IDomainEvent> event2Mock = new();
        Mock<IDomainEvent> event3Mock = new();

        // Act
        entity.AddTestDomainEvent(event1Mock.Object);
        entity.AddTestDomainEvent(event2Mock.Object);
        entity.AddTestDomainEvent(event3Mock.Object);

        // Assert
        using (new AssertionScope())
        {
            entity.DomainEvents[0].Should().Be(event1Mock.Object);
            entity.DomainEvents[1].Should().Be(event2Mock.Object);
            entity.DomainEvents[2].Should().Be(event3Mock.Object);
        }
    }
}