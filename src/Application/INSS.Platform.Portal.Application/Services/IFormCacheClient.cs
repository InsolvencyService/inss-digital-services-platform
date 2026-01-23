using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

/// <summary>
/// Provides methods for caching and retrieving form data and form lists.
/// </summary>
public interface IFormCacheClient
{
    /// <summary>
    /// Retrieves a form of type <typeparamref name="TForm"/> from the cache using the specified cache key.
    /// </summary>
    /// <typeparam name="TForm">The type of the form, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="cacheKey">The cache key used to retrieve the form.</param>
    /// <returns>The form instance if found; otherwise, <c>null</c>.</returns>
    TForm? GetFormFromCache<TForm>(string cacheKey) where TForm : FormBase, new();

    /// <summary>
    /// Retrieves a list of forms of type <typeparamref name="TForm"/> from the cache using the specified cache key.
    /// </summary>
    /// <typeparam name="TForm">The type of the forms, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="cacheKey">The cache key used to retrieve the form list.</param>
    /// <returns>A list of forms if found; otherwise, an empty list.</returns>
    List<TForm> GetFormListFromCache<TForm>(string cacheKey) where TForm : FormBase, new();

    /// <summary>
    /// Gets the current index of the form list from the cache using the specified cache key.
    /// </summary>
    /// <param name="cacheKey">The cache key used to retrieve the current index.</param>
    /// <returns>The current index of the form list.</returns>
    int GetCurrentFormListIndex(string cacheKey);

    /// <summary>
    /// Stores a form of type <typeparamref name="TForm"/> in the cache using the specified cache key.
    /// </summary>
    /// <typeparam name="TForm">The type of the form, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="cacheKey">The cache key used to store the form.</param>
    /// <param name="form">The form instance to store.</param>
    /// <returns>The stored form instance.</returns>
    TForm SetFormToCache<TForm>(string cacheKey, TForm form) where TForm : FormBase;

    /// <summary>
    /// Stores a list of forms of type <typeparamref name="TForm"/> in the cache using the specified cache key.
    /// </summary>
    /// <typeparam name="TForm">The type of the forms, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="cacheKey">The cache key used to store the form list.</param>
    /// <param name="forms">The collection of forms to store.</param>
    /// <returns>The stored list of forms.</returns>
    List<TForm> SetFormListToCache<TForm>(string cacheKey, IEnumerable<TForm> forms) where TForm : FormBase;

    /// <summary>
    /// Sets the current index of the form list in the cache using the specified cache key.
    /// </summary>
    /// <param name="cacheKey">The cache key used to store the current index.</param>
    /// <param name="currentIndex">The index to set as the current index.</param>
    void SetCurrentFormListIndex(string cacheKey, int currentIndex);

    /// <summary>
    /// Increments the current index of the form list in the cache using the specified cache key.
    /// </summary>
    /// <param name="cacheKey">The cache key used to increment the current index.</param>
    void IncrementCurrentFormListIndex(string cacheKey);

    /// <summary>
    /// Generates a cache key for a form of type <typeparamref name="TForm"/>, optionally using a suffix.
    /// </summary>
    /// <typeparam name="TForm">The type of the form, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="cacheKeySuffix">An optional suffix to append to the cache key.</param>
    /// <returns>The generated cache key string.</returns>
    string GetFormCacheKey<TForm>(string cacheKeySuffix = "") where TForm : FormBase;
}
