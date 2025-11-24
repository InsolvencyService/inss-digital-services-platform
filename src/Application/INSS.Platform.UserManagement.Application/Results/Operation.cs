namespace INSS.Platform.UserManagement.Application.Results;

/// <summary>
/// Provides static helper methods for creating <see cref="OperationResult{T}"/> instances
/// representing the outcome of an operation.
/// </summary>
public static class Operation
{
    /// <summary>
    /// Creates a successful <see cref="OperationResult{T}"/> with the specified entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity returned by the operation.</typeparam>
    /// <param name="entity">The entity to include in the result.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> indicating success, containing the provided entity and no error code.
    /// </returns>
    public static OperationResult<T> Ok<T>(T? entity) => new() { Success = true, Entity = entity, ErrorCode = OperationErrorCode.None };

    /// <summary>
    /// Creates a failed <see cref="OperationResult{T}"/> with the specified error message and error code.
    /// </summary>
    /// <typeparam name="T">The type of the entity that would have been returned by the operation.</typeparam>
    /// <param name="error">The error message describing the failure.</param>
    /// <param name="errorCode">The error code representing the type of failure.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> indicating failure, containing the provided error message and error code.
    /// </returns>
    public static OperationResult<T> Fail<T>(string error, OperationErrorCode errorCode) => new () { Success = false, ErrorMessage = error, ErrorCode = errorCode };
}
