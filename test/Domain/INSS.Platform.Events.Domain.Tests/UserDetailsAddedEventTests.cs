using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Events.Domain.Tests;

public class UserDetailsAddedEventTests
{
    [Fact]
    public void UserDetailsAddedEvent_CanBeCreated_WithAllProperties()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string fullName = "John Smith";
        DateOnly dateOfBirth = new(1980, 5, 15);
        string telephoneNumber = "07700900123";
        string emailAddress = "john.smith@example.com";

        // Act
        UserDetailsAddedEvent userEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            fullName,
            dateOfBirth,
            telephoneNumber,
            emailAddress);

        // Assert
        using (new AssertionScope())
        {
            userEvent.Actor.Should().Be(actor);
            userEvent.AggregateRootId.Should().Be(aggregateRootId);
            userEvent.CorrelationId.Should().Be(correlationId);
            userEvent.FullName.Should().Be(fullName);
            userEvent.DateOfBirth.Should().Be(dateOfBirth);
            userEvent.TelephoneNumber.Should().Be(telephoneNumber);
            userEvent.EmailAddress.Should().Be(emailAddress);
        }
    }

    [Fact]
    public void UserDetailsAddedEvent_InheritsFromDomainEvent()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        UserDetailsAddedEvent userEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            "Name",
            new(1990, 1, 1),
            "Phone",
            "Email");

        // Assert
        userEvent.Should().BeAssignableTo<DomainEvent>();
    }

    [Fact]
    public void UserDetailsAddedEvent_ImplementsIDomainEvent()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        UserDetailsAddedEvent userEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            "Name",
            new(1990, 1, 1),
            "Phone",
            "Email");

        // Assert
        userEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void UserDetailsAddedEvent_GeneratesEventId()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();

        // Act
        UserDetailsAddedEvent userEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            "Name",
            new(1990, 1, 1),
            "Phone",
            "Email");

        // Assert
        userEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void UserDetailsAddedEvent_IsSealed()
    {
        // Arrange & Act
        Type eventType = typeof(UserDetailsAddedEvent);

        // Assert
        eventType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void UserDetailsAddedEvent_IsRecord()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string fullName = "Jane Doe";
        DateOnly dateOfBirth = new(1985, 3, 20);
        string telephoneNumber = "07700900456";
        string emailAddress = "jane.doe@example.com";

        // Act
        UserDetailsAddedEvent event1 = new(actor, aggregateRootId, correlationId, fullName, dateOfBirth, telephoneNumber, emailAddress);
        UserDetailsAddedEvent event2 = new(actor, aggregateRootId, correlationId, fullName, dateOfBirth, telephoneNumber, emailAddress);

        // Assert - Records with same constructor values should have same property values
        using (new AssertionScope())
        {
            event1.FullName.Should().Be(event2.FullName);
            event1.DateOfBirth.Should().Be(event2.DateOfBirth);
            event1.TelephoneNumber.Should().Be(event2.TelephoneNumber);
            event1.EmailAddress.Should().Be(event2.EmailAddress);
        }
    }

    [Fact]
    public void UserDetailsAddedEvent_FullName_CanBeAccessed()
    {
        // Arrange
        string expectedFullName = "Alice Johnson";
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            expectedFullName,
            new(1995, 7, 10),
            "Phone",
            "Email");

        // Act
        string actualFullName = userEvent.FullName;

        // Assert
        actualFullName.Should().Be(expectedFullName);
    }

    [Fact]
    public void UserDetailsAddedEvent_DateOfBirth_CanBeAccessed()
    {
        // Arrange
        DateOnly expectedDateOfBirth = new(1992, 11, 25);
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Name",
            expectedDateOfBirth,
            "Phone",
            "Email");

        // Act
        DateOnly actualDateOfBirth = userEvent.DateOfBirth;

        // Assert
        actualDateOfBirth.Should().Be(expectedDateOfBirth);
    }

    [Fact]
    public void UserDetailsAddedEvent_TelephoneNumber_CanBeAccessed()
    {
        // Arrange
        string expectedTelephone = "01234567890";
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Name",
            new(1990, 1, 1),
            expectedTelephone,
            "Email");

        // Act
        string actualTelephone = userEvent.TelephoneNumber;

        // Assert
        actualTelephone.Should().Be(expectedTelephone);
    }

    [Fact]
    public void UserDetailsAddedEvent_EmailAddress_CanBeAccessed()
    {
        // Arrange
        string expectedEmail = "test.user@example.com";
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Name",
            new(1990, 1, 1),
            "Phone",
            expectedEmail);

        // Act
        string actualEmail = userEvent.EmailAddress;

        // Assert
        actualEmail.Should().Be(expectedEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Name")]
    public void UserDetailsAddedEvent_AcceptsVariousFullNameValues(string fullName)
    {
        // Arrange & Act
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            fullName,
            new(1990, 1, 1),
            "Phone",
            "Email");

        // Assert
        userEvent.FullName.Should().Be(fullName);
    }

    [Fact]
    public void UserDetailsAddedEvent_WithMinimumDateOfBirth_IsValid()
    {
        // Arrange
        DateOnly minDate = DateOnly.MinValue;

        // Act
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Name",
            minDate,
            "Phone",
            "Email");

        // Assert
        userEvent.DateOfBirth.Should().Be(minDate);
    }

    [Fact]
    public void UserDetailsAddedEvent_WithMaximumDateOfBirth_IsValid()
    {
        // Arrange
        DateOnly maxDate = DateOnly.MaxValue;

        // Act
        UserDetailsAddedEvent userEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Name",
            maxDate,
            "Phone",
            "Email");

        // Assert
        userEvent.DateOfBirth.Should().Be(maxDate);
    }
}