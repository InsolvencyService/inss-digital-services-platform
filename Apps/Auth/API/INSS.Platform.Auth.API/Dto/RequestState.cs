namespace INSS.Platform.Auth.API.Dto
{
    /// <summary>
    /// Represents the state of a request, including user and client information.
    /// </summary>
    public class RequestState
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user making the request.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the client application initiating the request.
        /// </summary>
        public string ClientUrl { get; set; } = string.Empty;
    }
}
