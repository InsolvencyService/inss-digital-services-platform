using FluentAssertions;
using FluentAssertions.Execution;

namespace INSS.Platform.Events.Domain.Tests;

public class UserIncomeAddedEventTests
{
    [Fact]
    public void UserIncomeAddedEvent_CanBeCreated_WithAllProperties()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string incomeProvider = "ABC Corporation";
        decimal grossIncome = 2500.50m;

        // Act
        UserIncomeAddedEvent incomeEvent = new(
            actor,
            aggregateRootId,
            correlationId,
            incomeProvider,
            grossIncome);

        // Assert
        using (new AssertionScope())
        {
            incomeEvent.Actor.Should().Be(actor);
            incomeEvent.AggregateRootId.Should().Be(aggregateRootId);
            incomeEvent.CorrelationId.Should().Be(correlationId);
            incomeEvent.IncomeProvider.Should().Be(incomeProvider);
            incomeEvent.GrossIncome.Should().Be(grossIncome);
        }
    }

    [Fact]
    public void UserIncomeAddedEvent_InheritsFromDomainEvent()
    {
        // Arrange & Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            1000m);

        // Assert
        incomeEvent.Should().BeAssignableTo<DomainEvent>();
    }

    [Fact]
    public void UserIncomeAddedEvent_ImplementsIDomainEvent()
    {
        // Arrange & Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            1000m);

        // Assert
        incomeEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void UserIncomeAddedEvent_GeneratesEventId()
    {
        // Arrange & Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            1000m);

        // Assert
        incomeEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void UserIncomeAddedEvent_IsSealed()
    {
        // Arrange & Act
        Type eventType = typeof(UserIncomeAddedEvent);

        // Assert
        eventType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void UserIncomeAddedEvent_IsRecord()
    {
        // Arrange
        string actor = "TestUser";
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        string incomeProvider = "XYZ Ltd";
        decimal grossIncome = 3000m;

        // Act
        UserIncomeAddedEvent event1 = new(actor, aggregateRootId, correlationId, incomeProvider, grossIncome);
        UserIncomeAddedEvent event2 = new(actor, aggregateRootId, correlationId, incomeProvider, grossIncome);

        // Assert
        using (new AssertionScope())
        {
            event1.IncomeProvider.Should().Be(event2.IncomeProvider);
            event1.GrossIncome.Should().Be(event2.GrossIncome);
        }
    }

    [Fact]
    public void UserIncomeAddedEvent_IncomeProvider_CanBeAccessed()
    {
        // Arrange
        string expectedProvider = "Test Company Ltd";
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            expectedProvider,
            1500m);

        // Act
        string actualProvider = incomeEvent.IncomeProvider;

        // Assert
        actualProvider.Should().Be(expectedProvider);
    }

    [Fact]
    public void UserIncomeAddedEvent_GrossIncome_CanBeAccessed()
    {
        // Arrange
        decimal expectedIncome = 4250.75m;
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            expectedIncome);

        // Act
        decimal actualIncome = incomeEvent.GrossIncome;

        // Assert
        actualIncome.Should().Be(expectedIncome);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(2500.99)]
    [InlineData(50000.00)]
    public void UserIncomeAddedEvent_AcceptsVariousGrossIncomeValues(decimal grossIncome)
    {
        // Arrange & Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            grossIncome);

        // Assert
        incomeEvent.GrossIncome.Should().Be(grossIncome);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Provider Name")]
    public void UserIncomeAddedEvent_AcceptsVariousIncomeProviderValues(string incomeProvider)
    {
        // Arrange & Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            incomeProvider,
            1000m);

        // Assert
        incomeEvent.IncomeProvider.Should().Be(incomeProvider);
    }

    [Fact]
    public void UserIncomeAddedEvent_GrossIncome_SupportsDecimalPrecision()
    {
        // Arrange
        decimal preciseIncome = 1234.567m;

        // Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            preciseIncome);

        // Assert
        incomeEvent.GrossIncome.Should().Be(preciseIncome);
    }

    [Fact]
    public void UserIncomeAddedEvent_WithNegativeIncome_CanBeCreated()
    {
        // Arrange
        decimal negativeIncome = -500m;

        // Act
        UserIncomeAddedEvent incomeEvent = new(
            "Actor",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Provider",
            negativeIncome);

        // Assert
        incomeEvent.GrossIncome.Should().Be(negativeIncome);
    }
}