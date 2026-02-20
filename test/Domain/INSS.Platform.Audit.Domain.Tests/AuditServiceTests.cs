using Moq;

namespace INSS.Platform.Audit.Domain.Tests;

public class AuditServiceTests
{
    [Fact]
    public async Task IAuditService_RecordAsync_CanBeCalled()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry auditEntry = new (
            "TestEvent",
            DateTime.UtcNow,
            "Test description",
            null);
        CancellationToken cancellationToken = CancellationToken.None;

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(auditEntry, cancellationToken);

        // Assert
        auditServiceMock.Verify(
            x => x.RecordAsync(auditEntry, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task IAuditService_RecordAsync_PassesCorrectAuditEntry()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        string eventType = "UserRegistered";
        DateTime timestamp = DateTime.UtcNow;
        string description = "New user registration";
        object metadata = new { UserId = "USER-001", Email = "user@example.com" };

        AuditEntry expectedEntry = new (eventType, timestamp, description, metadata);
        AuditEntry? capturedEntry = null;

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Callback<AuditEntry, CancellationToken>((entry, ct) => capturedEntry = entry)
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(expectedEntry, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEntry);
        Assert.Equal(expectedEntry.EventType, capturedEntry.EventType);
        Assert.Equal(expectedEntry.TimestampUtc, capturedEntry.TimestampUtc);
        Assert.Equal(expectedEntry.Description, capturedEntry.Description);
        Assert.Equal(expectedEntry.Metadata, capturedEntry.Metadata);
    }

    [Fact]
    public async Task IAuditService_RecordAsync_PassesCancellationToken()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry auditEntry = new ("Event", DateTime.UtcNow, "Description", null);
        CancellationTokenSource cts = new ();
        CancellationToken cancellationToken = cts.Token;
        CancellationToken capturedToken = default;

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Callback<AuditEntry, CancellationToken>((entry, ct) => capturedToken = ct)
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(auditEntry, cancellationToken);

        // Assert
        Assert.Equal(cancellationToken, capturedToken);
    }

    [Fact]
    public async Task IAuditService_RecordAsync_CanHandleCancellation()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry auditEntry = new ("Event", DateTime.UtcNow, "Description", null);
        CancellationTokenSource cts = new ();
        cts.Cancel();

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await auditServiceMock.Object.RecordAsync(auditEntry, cts.Token));
    }

    [Fact]
    public async Task IAuditService_RecordAsync_WithNullMetadata_Succeeds()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry auditEntry = new (
            "SimpleEvent",
            DateTime.UtcNow,
            "Event without metadata",
            null);

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(auditEntry, CancellationToken.None);

        // Assert
        auditServiceMock.Verify(
            x => x.RecordAsync(It.Is<AuditEntry>(e => e.Metadata == null), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task IAuditService_RecordAsync_WithComplexMetadata_Succeeds()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        Dictionary<string, object> complexMetadata = new ()
        {
            { "RequestId", Guid.NewGuid() },
            { "HttpMethod", "POST" },
            { "StatusCode", 200 },
            { "ResponseTime", TimeSpan.FromMilliseconds(150) }
        };

        AuditEntry auditEntry = new (
            "ApiCall",
            DateTime.UtcNow,
            "API endpoint was called",
            complexMetadata);

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(auditEntry, CancellationToken.None);

        // Assert
        auditServiceMock.Verify(
            x => x.RecordAsync(
                It.Is<AuditEntry>(e => e.Metadata == complexMetadata),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task IAuditService_RecordAsync_CanBeCalledMultipleTimes()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry entry1 = new ("Event1", DateTime.UtcNow, "Description1", null);
        AuditEntry entry2 = new ("Event2", DateTime.UtcNow, "Description2", null);
        AuditEntry entry3 = new ("Event3", DateTime.UtcNow, "Description3", null);

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await auditServiceMock.Object.RecordAsync(entry1, CancellationToken.None);
        await auditServiceMock.Object.RecordAsync(entry2, CancellationToken.None);
        await auditServiceMock.Object.RecordAsync(entry3, CancellationToken.None);

        // Assert
        auditServiceMock.Verify(
            x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task IAuditService_RecordAsync_CanThrowException()
    {
        // Arrange
        Mock<IAuditService> auditServiceMock = new ();
        AuditEntry auditEntry = new ("Event", DateTime.UtcNow, "Description", null);
        InvalidOperationException expectedException = new ("Audit storage unavailable");

        auditServiceMock
            .Setup(x => x.RecordAsync(It.IsAny<AuditEntry>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await auditServiceMock.Object.RecordAsync(auditEntry, CancellationToken.None));

        Assert.Equal("Audit storage unavailable", exception.Message);
    }
}
