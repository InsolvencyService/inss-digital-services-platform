using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Tests;

public class OperationTests
{
    [Fact]
    public void Ok_WithEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        RelationshipType entity = new ()
        {
            Id = Guid.NewGuid(),
            Name = "Parent-Child",
            Description = "Parent to child relationship"
        };

        // Act
        OperationResult<RelationshipType> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithNullEntity_ReturnsSuccessfulResultWithNullEntity()
    {
        // Arrange & Act
        OperationResult<RelationshipType> result = Operation.Ok<RelationshipType>(null);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithStringEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        string entity = "Test String";

        // Act
        OperationResult<string> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithIntEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        int entity = 42;

        // Act
        OperationResult<int> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithGuidEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        Guid entity = Guid.NewGuid();

        // Act
        OperationResult<Guid> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithListEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        List<string> entity = new () { "Item1", "Item2", "Item3" };

        // Act
        OperationResult<List<string>> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Equal(3, result.Entity.Count);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Fail_WithErrorMessageAndCode_ReturnsFailedResult()
    {
        // Arrange
        string errorMessage = "Entity not found";
        OperationErrorCode errorCode = OperationErrorCode.NotFound;

        // Act
        OperationResult<RelationshipType> result = Operation.Fail<RelationshipType>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(errorCode, result.ErrorCode);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Fail_WithSqlError_ReturnsFailedResult()
    {
        // Arrange
        string errorMessage = "Database connection failed";
        OperationErrorCode errorCode = OperationErrorCode.SqlError;

        // Act
        OperationResult<string> result = Operation.Fail<string>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(OperationErrorCode.SqlError, result.ErrorCode);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Fail_WithValidationError_ReturnsFailedResult()
    {
        // Arrange
        string errorMessage = "Validation failed for required fields";
        OperationErrorCode errorCode = OperationErrorCode.Unknown;

        // Act
        OperationResult<RelationshipType> result = Operation.Fail<RelationshipType>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(OperationErrorCode.Unknown, result.ErrorCode);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Fail_WithEmptyErrorMessage_ReturnsFailedResult()
    {
        // Arrange
        string errorMessage = string.Empty;
        OperationErrorCode errorCode = OperationErrorCode.NotFound;

        // Act
        OperationResult<RelationshipType> result = Operation.Fail<RelationshipType>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(errorCode, result.ErrorCode);
        Assert.Equal(string.Empty, result.ErrorMessage);
    }

    [Theory]
    [InlineData(OperationErrorCode.None)]
    [InlineData(OperationErrorCode.NotFound)]
    [InlineData(OperationErrorCode.SqlError)]
    [InlineData(OperationErrorCode.Unknown)]
    public void Fail_WithVariousErrorCodes_ReturnsFailedResultWithCorrectErrorCode(OperationErrorCode errorCode)
    {
        // Arrange
        string errorMessage = $"Error occurred with code {errorCode}";

        // Act
        OperationResult<string> result = Operation.Fail<string>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(errorCode, result.ErrorCode);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Ok_WithComplexEntity_ReturnsSuccessfulResult()
    {
        // Arrange
        Address entity = new ()
        {
            Id = Guid.NewGuid(),
            AddressLine1 = "123 Main Street",
            AddressLine2 = "Apt 4B",
            AddressLine3 = "London",
            Postcode = "SW1A 1AA"
        };

        // Act
        OperationResult<Address> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Equal(entity.Id, result.Entity.Id);
        Assert.Equal(entity.AddressLine1, result.Entity.AddressLine1);
        Assert.Equal(entity.AddressLine2, result.Entity.AddressLine2);
        Assert.Equal(entity.AddressLine3, result.Entity.AddressLine3);
        Assert.Equal(entity.Postcode, result.Entity.Postcode);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Fail_WithLongErrorMessage_ReturnsFailedResult()
    {
        // Arrange
        string errorMessage = "This is a very long error message that contains detailed information " +
                             "about what went wrong in the system, including stack traces and " +
                             "additional context that might be useful for debugging purposes.";
        OperationErrorCode errorCode = OperationErrorCode.SqlError;

        // Act
        OperationResult<RelationshipType> result = Operation.Fail<RelationshipType>(errorMessage, errorCode);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(errorCode, result.ErrorCode);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

    [Fact]
    public void Ok_MultipleCallsWithSameEntity_ReturnsDifferentResultInstances()
    {
        // Arrange
        string entity = "Test";

        // Act
        OperationResult<string> result1 = Operation.Ok(entity);
        OperationResult<string> result2 = Operation.Ok(entity);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(result1.Entity, result2.Entity);
        Assert.Equal(result1.Success, result2.Success);
        Assert.Equal(result1.ErrorCode, result2.ErrorCode);
    }

    [Fact]
    public void Fail_MultipleCallsWithSameParameters_ReturnsDifferentResultInstances()
    {
        // Arrange
        string errorMessage = "Test error";
        OperationErrorCode errorCode = OperationErrorCode.NotFound;

        // Act
        OperationResult<string> result1 = Operation.Fail<string>(errorMessage, errorCode);
        OperationResult<string> result2 = Operation.Fail<string>(errorMessage, errorCode);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(result1.ErrorMessage, result2.ErrorMessage);
        Assert.Equal(result1.Success, result2.Success);
        Assert.Equal(result1.ErrorCode, result2.ErrorCode);
    }

    [Fact]
    public void Ok_WithNullableValueType_ReturnsSuccessfulResult()
    {
        // Arrange
        int? entity = 123;

        // Act
        OperationResult<int?> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Entity);
        Assert.Equal(entity, result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
    }

    [Fact]
    public void Ok_WithNullableValueTypeAsNull_ReturnsSuccessfulResultWithNull()
    {
        // Arrange
        int? entity = null;

        // Act
        OperationResult<int?> result = Operation.Ok(entity);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Entity);
        Assert.Equal(OperationErrorCode.None, result.ErrorCode);
    }
}