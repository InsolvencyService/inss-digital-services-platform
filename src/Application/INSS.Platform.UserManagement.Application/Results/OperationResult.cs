namespace INSS.Platform.UserManagement.Application.Results;

/// <summary>
/// Represents the result of an operation, including success status, entity, and error information.
/// </summary>
/// <typeparam name="T">The type of the entity returned by the operation.</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the entity returned by the operation, if successful.
    /// </summary>
    public T? Entity { get; set; }

    /// <summary>
    /// Gets or sets the error code associated with the operation result.
    /// </summary>
    public OperationErrorCode ErrorCode { get; set; } = OperationErrorCode.None;

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}