using INSS.Platform.Auth.API.Dto;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines methods for storing, retrieving, and removing state objects in a cache.
    /// </summary>
    public interface IStateCache
    {
        /// <summary>
        /// Stores a <see cref="RequestState"/> object in the cache using the specified key.
        /// </summary>
        /// <param name="stateKey">The key to associate with the state object.</param>
        /// <param name="value">The <see cref="RequestState"/> object to store.</param>
        void Store(string stateKey, RequestState value);

        /// <summary>
        /// Attempts to retrieve a <see cref="RequestState"/> object from the cache using the specified key.
        /// If found, the object is then removed from the cache.
        /// </summary>
        /// <param name="stateKey">The key associated with the state object.</param>
        /// <param name="value">
        /// When this method returns, contains the retrieved <see cref="RequestState"/> object if found; otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the state object was found; otherwise, <c>false</c>.
        /// </returns>
        bool TryGet(string stateKey, out RequestState value);

        /// <summary>
        /// Removes expired state objects from the cache.
        /// </summary>
        void CleanupExpired();
    }
}
