using INSS.Platform.Canonical.Domain;

namespace INSS.Platform.Portal.Application.Clients;

/// <summary>
/// Defines methods for interacting with the Canonical Data API.
/// </summary>
public interface ICanonicalDataClient
{
    /// <summary>
    /// Posts the specified user data to the Canonical Data API endpoint asynchronously.
    /// </summary>
    /// <param name="userData">The user data to be posted.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains <c>true</c> if the post was successful; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> PostUserDataAsync(User userData);
}
