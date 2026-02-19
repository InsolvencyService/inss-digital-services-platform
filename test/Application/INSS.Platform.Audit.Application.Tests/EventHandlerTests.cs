using FluentAssertions.Execution;
using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Audit.Application.Users.Handlers;
using INSS.Platform.Audit.Domain;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Events.Domain;
using Moq;

namespace INSS.Platform.Audit.Application.Tests;

public class EventHandlerTests
{
    private Mock<IAuditService> _auditServiceMock;

    [Fact]
    public void AddUserBankDetailsHandler_AddsDomainEvent()
    {
        Guid correlationId = Guid.NewGuid();
        User user = new() { FullName = "test name", DateOfBirth = new DateOnly(1999, 12, 31), TelephoneNumber = "0800 999999", EmailAddress = "test@domain.com" };
        AddUserBankDetailsCommand command = new() { User = "test user", AccountName = "test account", SortCode = "12-34-56", CorrelationId = correlationId };

        AddUserBankDetailsHandler.Handle(user, command);

        using (new AssertionScope())
        {
            Assert.Single(user.DomainEvents);
            Assert.IsType<UserBankDetailsAddedEvent>(user.DomainEvents[0]);
            Assert.Equal("test account", ((UserBankDetailsAddedEvent)user.DomainEvents[0]).AccountName);
            Assert.Equal("12-34-56", ((UserBankDetailsAddedEvent)user.DomainEvents[0]).SortCode);
            Assert.Equal(correlationId, ((UserBankDetailsAddedEvent)user.DomainEvents[0]).CorrelationId);
        }
    }

    [Fact]
    public async Task UserBankDetailsAddedHandler_RecordsEvent()
    {
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        UserBankDetailsAddedEvent domainEvent = new("System", aggregateRootId, correlationId, "test account", "12-34-56");

        _auditServiceMock = new Mock<IAuditService>();
        UserBankDetailsAddedHandler handler = new(_auditServiceMock.Object);

        await handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        _auditServiceMock.Verify(a => a.RecordAsync(
            It.Is<AuditEntry>(e =>
                e.EventType == "UserBankDetailsAddedEvent" 
                && e.Metadata is UserBankDetailsAddedEvent 
                && ((UserBankDetailsAddedEvent)e.Metadata).AccountName == "test account"
                && ((UserBankDetailsAddedEvent)e.Metadata).SortCode == "12-34-56"
                && ((UserBankDetailsAddedEvent)e.Metadata).Actor == "System"
                && ((UserBankDetailsAddedEvent)e.Metadata).AggregateRootId == aggregateRootId
                && ((UserBankDetailsAddedEvent)e.Metadata).CorrelationId == correlationId

            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void AddUserDetailsHandler_AddsDomainEvent()
    {
        Guid correlationId = Guid.NewGuid();
        User user = new() { FullName = "test name", DateOfBirth = new DateOnly(1999, 12, 31), TelephoneNumber = "0800 999999", EmailAddress = "test@domain.com" };
        AddUserDetailsCommand command = new() { User = "test user", FullName = "test name", DateOfBirth = new DateOnly(1999, 12, 31), TelephoneNumber = "0800 999999", EmailAddress = "test@domain.com", CorrelationId = correlationId };

        AddUserDetailsHandler.Handle(user, command);

        using (new AssertionScope())
        {
            Assert.Single(user.DomainEvents);
            Assert.IsType<UserDetailsAddedEvent>(user.DomainEvents[0]);
            Assert.Equal("test name", ((UserDetailsAddedEvent)user.DomainEvents[0]).FullName);
            Assert.Equal(new DateOnly(1999, 12, 31), ((UserDetailsAddedEvent)user.DomainEvents[0]).DateOfBirth);
            Assert.Equal("0800 999999", ((UserDetailsAddedEvent)user.DomainEvents[0]).TelephoneNumber);
            Assert.Equal("test@domain.com", ((UserDetailsAddedEvent)user.DomainEvents[0]).EmailAddress);
            Assert.Equal(correlationId, ((UserDetailsAddedEvent)user.DomainEvents[0]).CorrelationId);
        }
    }

    [Fact]
    public async Task UserDetailsAddedHandler_RecordsEvent()
    {
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        UserDetailsAddedEvent domainEvent = new("System", aggregateRootId, correlationId, "test name", new DateOnly(1999, 12, 31), "0800 999999", "test@domain.com");

        _auditServiceMock = new Mock<IAuditService>();
        UserDetailsAddedHandler handler = new(_auditServiceMock.Object);

        await handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        _auditServiceMock.Verify(a => a.RecordAsync(
            It.Is<AuditEntry>(e =>
                e.EventType == "UserDetailsAddedEvent"
                && e.Metadata is UserDetailsAddedEvent
                && ((UserDetailsAddedEvent)e.Metadata).FullName == "test name"
                && ((UserDetailsAddedEvent)e.Metadata).DateOfBirth == new DateOnly(1999, 12, 31)
                && ((UserDetailsAddedEvent)e.Metadata).TelephoneNumber == "0800 999999"
                && ((UserDetailsAddedEvent)e.Metadata).EmailAddress == "test@domain.com"
                && ((UserDetailsAddedEvent)e.Metadata).Actor == "System"
                && ((UserDetailsAddedEvent)e.Metadata).AggregateRootId == aggregateRootId
                && ((UserDetailsAddedEvent)e.Metadata).CorrelationId == correlationId

            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void AddUserIncomeHandler_AddsDomainEvent()
    {
        Guid correlationId = Guid.NewGuid();
        User user = new() { FullName = "test name", DateOfBirth = new DateOnly(1999, 12, 31), TelephoneNumber = "0800 999999", EmailAddress = "test@domain.com" };
        AddUserIncomeCommand command = new() { User = "test user", IncomeProvider = "test provider", GrossIncome = 99.999m, CorrelationId = correlationId };

        AddUserIncomeHandler.Handle(user, command);

        using (new AssertionScope())
        {
            Assert.Single(user.DomainEvents);
            Assert.IsType<UserIncomeAddedEvent>(user.DomainEvents[0]);
            Assert.Equal("test provider", ((UserIncomeAddedEvent)user.DomainEvents[0]).IncomeProvider);
            Assert.Equal(99.999m, ((UserIncomeAddedEvent)user.DomainEvents[0]).GrossIncome);
            Assert.Equal(correlationId, ((UserIncomeAddedEvent)user.DomainEvents[0]).CorrelationId);
        }
    }

    [Fact]
    public async Task UserIncomeAddedHandler_RecordsEvent()
    {
        Guid aggregateRootId = Guid.NewGuid();
        Guid correlationId = Guid.NewGuid();
        UserIncomeAddedEvent domainEvent = new("System", aggregateRootId, correlationId, "test provider", 999.999m);

        _auditServiceMock = new Mock<IAuditService>();
        UserIncomeAddedHandler handler = new(_auditServiceMock.Object);

        await handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        _auditServiceMock.Verify(a => a.RecordAsync(
            It.Is<AuditEntry>(e =>
                e.EventType == "UserIncomeAddedEvent"
                && e.Metadata is UserIncomeAddedEvent
                && ((UserIncomeAddedEvent)e.Metadata).IncomeProvider == "test provider"
                && ((UserIncomeAddedEvent)e.Metadata).GrossIncome == 999.999m
                && ((UserIncomeAddedEvent)e.Metadata).Actor == "System"
                && ((UserIncomeAddedEvent)e.Metadata).AggregateRootId == aggregateRootId
                && ((UserIncomeAddedEvent)e.Metadata).CorrelationId == correlationId

            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

}
