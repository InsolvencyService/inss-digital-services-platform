namespace INSS.Platform.UserManagement.Abstractions.Results
{
    /// <summary>
    /// Represents error codes for user management operations.
    /// </summary>
    public enum OperationErrorCode
    {
        /// <summary>
        /// No error occurred.
        /// </summary>
        None = 0,

        /// <summary>
        /// The requested resource was not found.
        /// </summary>
        NotFound = 1,

        /// <summary>
        /// An error occurred while accessing the SQL database.
        /// </summary>
        SqlError = 2,

        /// <summary>
        /// An unknown error occurred.
        /// </summary>
        Unknown = 99
    }
}
