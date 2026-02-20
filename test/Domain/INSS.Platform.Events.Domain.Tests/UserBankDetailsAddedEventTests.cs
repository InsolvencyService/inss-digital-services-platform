using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Events.Domain.Tests;

public class UserBankDetailsAddedEventTests
{
    [Fact]
    public void UserBankDetailsAddedEvent_CanBeCreated_WithAllProperties()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string accountName = "Mr John Smith";
        string sortCode = "12-34-56";

        // Act
        UserBankDetailsAddedEvent bankEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            accountName,
            sortCode);

        // Assert
        using (new AssertionScope())
        {
            bankEvent.Actor.Should().Be(actor);
            bankEvent.AggregateRootId.Should().Be(aggregateRootId);
            bankEvent.CorrelationId.Should().Be(correlationId);
            bankEvent.AccountName.Should().Be(accountName);
            bankEvent.SortCode.Should().Be(sortCode);
        }
    }

    [Fact]
    public void UserBankDetailsAddedEvent_InheritsFromDomainEvent()
    {
        // Arrange & Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            "SortCode");

        // Assert
        bankEvent.Should().BeAssignableTo<DomainEvent>();
    }

    [Fact]
    public void UserBankDetailsAddedEvent_ImplementsIDomainEvent()
    {
        // Arrange & Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            "SortCode");

        // Assert
        bankEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void UserBankDetailsAddedEvent_GeneratesEventId()
    {
        // Arrange & Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            "SortCode");

        // Assert
        bankEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void UserBankDetailsAddedEvent_IsSealed()
    {
        // Arrange & Act
        Type eventType = typeof(UserBankDetailsAddedEvent);

        // Assert
        eventType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void UserBankDetailsAddedEvent_IsRecord()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string accountName = "Test Account";
        string sortCode = "20-00-00";

        // Act
        UserBankDetailsAddedEvent event1 = new(actor, aggregateRootId, correlationId, accountName, sortCode);
        UserBankDetailsAddedEvent event2 = new(actor, aggregateRootId, correlationId, accountName, sortCode);

        // Assert
        using (new AssertionScope())
        {
            event1.AccountName.Should().Be(event2.AccountName);
            event1.SortCode.Should().Be(event2.SortCode);
        }
    }

    [Fact]
    public void UserBankDetailsAddedEvent_AccountName_CanBeAccessed()
    {
        // Arrange
        string expectedAccountName = "Ms Jane Doe";
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            expectedAccountName,
            "SortCode");

        // Act
        string actualAccountName = bankEvent.AccountName;

        // Assert
        actualAccountName.Should().Be(expectedAccountName);
    }

    [Fact]
    public void UserBankDetailsAddedEvent_SortCode_CanBeAccessed()
    {
        // Arrange
        string expectedSortCode = "40-47-84";
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            expectedSortCode);

        // Act
        string actualSortCode = bankEvent.SortCode;

        // Assert
        actualSortCode.Should().Be(expectedSortCode);
    }

    [Theory]
    [InlineData("12-34-56")]
    [InlineData("20-00-00")]
    [InlineData("40-47-84")]
    [InlineData("08-92-99")]
    public void UserBankDetailsAddedEvent_AcceptsVariousSortCodeFormats(string sortCode)
    {
        // Arrange & Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            sortCode);

        // Assert
        bankEvent.SortCode.Should().Be(sortCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Account Name")]
    public void UserBankDetailsAddedEvent_AcceptsVariousAccountNameValues(string accountName)
    {
        // Arrange & Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            accountName,
            "SortCode");

        // Assert
        bankEvent.AccountName.Should().Be(accountName);
    }

    [Fact]
    public void UserBankDetailsAddedEvent_WithLongAccountName_IsValid()
    {
        // Arrange
        string longAccountName = "Dr Professor Sir John William Alexander Smith-Jones III Esq.";

        // Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            longAccountName,
            "SortCode");

        // Assert
        bankEvent.AccountName.Should().Be(longAccountName);
    }

    [Fact]
    public void UserBankDetailsAddedEvent_WithoutDashesInSortCode_IsValid()
    {
        // Arrange
        string sortCodeWithoutDashes = "123456";

        // Act
        UserBankDetailsAddedEvent bankEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Account",
            sortCodeWithoutDashes);

        // Assert
        bankEvent.SortCode.Should().Be(sortCodeWithoutDashes);
    }
}