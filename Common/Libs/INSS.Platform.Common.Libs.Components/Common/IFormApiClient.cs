namespace INSS.Platform.Common.Libs.Components.Common
{
    /// <summary>
    /// Defines a contract for an API client that can post form data to a specified API endpoint.
    /// </summary>
    public interface IFormApiClient
    {
        /// <summary>
        /// Asynchronously posts form data to the specified API URL.
        /// </summary>
        /// <param name="formData">The form data to be posted, represented by a <see cref="FormBase"/> instance.</param>
        /// <param name="apiUrl">The URL of the API endpoint to which the form data will be posted.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
        /// with a result of <c>true</c> if the post was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> PostFormDataAsync(FormBase formData, string apiUrl);
    }
}
