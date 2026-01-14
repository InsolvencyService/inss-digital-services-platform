using INSS.Platform.Canonical.Domain;

namespace INSS.Platform.AlphaDemo.Web.Services;

/// <summary>
/// Defines a contract for an API client that can post user data to a specified API endpoint.
/// </summary>
public interface IFormApiClient
{
    /// <summary>
    /// Posts the specified user data to the API endpoint asynchronously.
    /// </summary>
    /// <param name="userData">The user data to be posted.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains <c>true</c> if the post was successful; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> PostFormUserDataAsync(User userData);
}
