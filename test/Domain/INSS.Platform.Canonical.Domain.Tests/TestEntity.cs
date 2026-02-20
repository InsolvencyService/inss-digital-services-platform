using INSS.Platform.Events.Domain;

namespace INSS.Platform.Canonical.Domain.Tests;

internal sealed class TestEntity : BaseEntity
{
    public void AddTestDomainEvent(IDomainEvent domainEvent)
    {
        AddDomainEvent(domainEvent);
    }
}