using INSS.Platform.Portal.Domain.Abstract;

namespace INSS.Platform.Portal.Application.Clients;

/// <summary>
/// Provides methods for caching and retrieving form data and form lists.
/// </summary>
public interface IFormCacheClient
{
    /// <summary>
    /// Retrieves a form of type <typeparamref name="TForm"/> from the cache using the specified cache key.
    /// </summary>
    /// <typeparam name="TForm">The type of the form, derived from <see cref="FormBase"/>.</typeparam>
    /// <returns>The form instance if found; otherwise, <c>null</c>.</returns>
    Task<TForm> GetFormFromCacheAsync<TForm>() where TForm : FormBase, new();

    /// <summary>
    /// Retrieves a form list of type <typeparamref name="TFormList"/> from the cache.
    /// </summary>
    /// <typeparam name="TFormList">The type of the form list, derived from <see cref="FormBase"/>.</typeparam>
    /// <returns>The form list instance if found; otherwise, <c>null</c>.</returns>
    Task<TFormList> GetFormListFromCacheAsync<TFormList>() where TFormList : FormBase, new();

    /// <summary>
    /// Gets the current index of the form list from the cache.
    /// </summary>
    /// <returns>The current index of the form list.</returns>
    Task<int> GetCurrentFormListIndexAsync();

    /// <summary>
    /// Stores a form of type <typeparamref name="TForm"/> in the cache.
    /// </summary>
    /// <typeparam name="TForm">The type of the form, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="form">The form instance to store.</param>
    /// <returns>A task representing the asynchronous operation, with a boolean result indicating success or failure.</returns>
    Task<bool> SaveFormToCacheAsync<TForm>(TForm form) where TForm : FormBase;

    /// <summary>
    /// Stores a form list of type <typeparamref name="TFormList"/> in the cache.
    /// </summary>
    /// <typeparam name="TFormList">The type of the form list, derived from <see cref="FormBase"/>.</typeparam>
    /// <param name="fromList">The form list instance to store.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating success or failure.
    /// </returns>
    Task<bool> SaveFormListToCacheAsync<TFormList>(TFormList fromList) where TFormList : FormBase;

    /// <summary>
    /// Sets the current index of the form list in the cache.
    /// </summary>
    /// <param name="currentIndex">The index to set as the current index.</param>
    Task SetCurrentFormListIndexAsync(int currentIndex);

    /// <summary>
    /// Increments the current index of the form list in the cache.
    /// </summary>
    Task IncrementCurrentFormListIndexAsync();
}
