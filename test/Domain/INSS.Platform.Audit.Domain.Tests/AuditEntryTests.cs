namespace INSS.Platform.Audit.Domain.Tests;

public class AuditEntryTests
{
    [Fact]
    public void AuditEntry_CanBeCreated_WithAllProperties()
    {
        // Arrange
        string eventType = "UserCreated";
        DateTime timestamp = DateTime.UtcNow;
        string description = "A new user was created";
        object metadata = new { UserId = "123", UserName = "John Doe" };

        // Act
        AuditEntry entry = new (eventType, timestamp, description, metadata);

        // Assert
        Assert.Equal(eventType, entry.EventType);
        Assert.Equal(timestamp, entry.TimestampUtc);
        Assert.Equal(description, entry.Description);
        Assert.Equal(metadata, entry.Metadata);
    }

    [Fact]
    public void AuditEntry_CanBeCreated_WithNullMetadata()
    {
        // Arrange
        string eventType = "UserDeleted";
        DateTime timestamp = DateTime.UtcNow;
        string description = "User was deleted";

        // Act
        AuditEntry entry = new (eventType, timestamp, description, null);

        // Assert
        Assert.Equal(eventType, entry.EventType);
        Assert.Equal(timestamp, entry.TimestampUtc);
        Assert.Equal(description, entry.Description);
        Assert.Null(entry.Metadata);
    }

    [Fact]
    public void AuditEntry_WithSameValues_AreEqual()
    {
        // Arrange
        string eventType = "OrderPlaced";
        DateTime timestamp = new (2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        string description = "Order was placed";
        object metadata = new { OrderId = "456" };

        AuditEntry entry1 = new (eventType, timestamp, description, metadata);
        AuditEntry entry2 = new (eventType, timestamp, description, metadata);

        // Act & Assert
        Assert.Equal(entry1, entry2);
        Assert.True(entry1 == entry2);
        Assert.False(entry1 != entry2);
    }

    [Fact]
    public void AuditEntry_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        DateTime timestamp = DateTime.UtcNow;
        object metadata = new { Data = "test" };

        AuditEntry entry1 = new ("EventType1", timestamp, "Description1", metadata);
        AuditEntry entry2 = new ("EventType2", timestamp, "Description2", metadata);

        // Act & Assert
        Assert.NotEqual(entry1, entry2);
        Assert.False(entry1 == entry2);
        Assert.True(entry1 != entry2);
    }

    [Fact]
    public void AuditEntry_GetHashCode_ReturnsSameValueForEqualRecords()
    {
        // Arrange
        string eventType = "DataExported";
        DateTime timestamp = DateTime.UtcNow;
        string description = "Data was exported";
        object metadata = new { Format = "CSV" };

        AuditEntry entry1 = new (eventType, timestamp, description, metadata);
        AuditEntry entry2 = new (eventType, timestamp, description, metadata);

        // Act
        int hash1 = entry1.GetHashCode();
        int hash2 = entry2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void AuditEntry_ToString_ReturnsFormattedString()
    {
        // Arrange
        string eventType = "LoginAttempt";
        DateTime timestamp = DateTime.UtcNow;
        string description = "User login attempt";
        object metadata = new { IpAddress = "192.168.1.1" };

        AuditEntry entry = new (eventType, timestamp, description, metadata);

        // Act
        string result = entry.ToString();

        // Assert
        Assert.NotNull(result);
        Assert.Contains("AuditEntry", result);
        Assert.Contains(eventType, result);
    }

    [Fact]
    public void AuditEntry_CanBeDeconstructed()
    {
        // Arrange
        string expectedEventType = "PasswordChanged";
        DateTime expectedTimestamp = DateTime.UtcNow;
        string expectedDescription = "User changed password";
        object expectedMetadata = new { UserId = "789" };

        AuditEntry entry = new (expectedEventType, expectedTimestamp, expectedDescription, expectedMetadata);

        // Act
        (string eventType, DateTime timestamp, string description, object? metadata) = entry;

        // Assert
        Assert.Equal(expectedEventType, eventType);
        Assert.Equal(expectedTimestamp, timestamp);
        Assert.Equal(expectedDescription, description);
        Assert.Equal(expectedMetadata, metadata);
    }

    [Fact]
    public void AuditEntry_With_CreatesNewInstanceWithModifiedProperty()
    {
        // Arrange
        string originalEventType = "OriginalEvent";
        DateTime timestamp = DateTime.UtcNow;
        string description = "Original description";
        object metadata = new { Value = 1 };

        AuditEntry original = new (originalEventType, timestamp, description, metadata);
        string newEventType = "ModifiedEvent";

        // Act
        AuditEntry modified = original with { EventType = newEventType };

        // Assert
        Assert.NotEqual(original, modified);
        Assert.Equal(newEventType, modified.EventType);
        Assert.Equal(timestamp, modified.TimestampUtc);
        Assert.Equal(description, modified.Description);
        Assert.Equal(metadata, modified.Metadata);
    }

    [Fact]
    public void AuditEntry_WithComplexMetadata_StoresCorrectly()
    {
        // Arrange
        string eventType = "TransactionCompleted";
        DateTime timestamp = DateTime.UtcNow;
        string description = "Transaction completed successfully";
        Dictionary<string, object> metadata = new ()
        {
            { "TransactionId", "TXN-12345" },
            { "Amount", 150.50m },
            { "Currency", "GBP" },
            { "Items", new List<string> { "Item1", "Item2", "Item3" } }
        };

        // Act
        AuditEntry entry = new (eventType, timestamp, description, metadata);

        // Assert
        Assert.Equal(metadata, entry.Metadata);
        Dictionary<string, object>? retrievedMetadata = entry.Metadata as Dictionary<string, object>;
        Assert.NotNull(retrievedMetadata);
        Assert.Equal("TXN-12345", retrievedMetadata["TransactionId"]);
        Assert.Equal(150.50m, retrievedMetadata["Amount"]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ValidEventType")]
    public void AuditEntry_AcceptsVariousEventTypeValues(string eventType)
    {
        // Arrange
        DateTime timestamp = DateTime.UtcNow;
        string description = "Test description";

        // Act
        AuditEntry entry = new (eventType, timestamp, description, null);

        // Assert
        Assert.Equal(eventType, entry.EventType);
    }

    [Fact]
    public void AuditEntry_WithUtcTimestamp_PreservesTimestamp()
    {
        // Arrange
        DateTime utcTimestamp = new (2024, 2, 20, 14, 30, 0, DateTimeKind.Utc);
        string eventType = "SystemStartup";
        string description = "System was started";

        // Act
        AuditEntry entry = new (eventType, utcTimestamp, description, null);

        // Assert
        Assert.Equal(utcTimestamp, entry.TimestampUtc);
        Assert.Equal(DateTimeKind.Utc, entry.TimestampUtc.Kind);
    }
}
