using INSS.Platform.Portal.Domain.Abstract;
using System.Net;

namespace INSS.Platform.Cache.Application.Repositories;

/// <summary>
/// Defines methods for saving and retrieving cached data.
/// </summary>
public interface ICacheRepository
{
    /// <summary>
    /// Retrieves a form object from the cache asynchronously by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the form to retrieve from the cache.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the <see cref="FormBase"/> object retrieved from the cache, or <c>null</c> if not found.
    /// </returns>
    Task<TForm?> GetFormAsync<TForm>(Guid id) where TForm : FormBase;

    /// <summary>
    /// Saves a form object to the cache.
    /// </summary>
    /// <param name="form">The form object to be saved in the cache.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the HTTP status code indicating the result of the save operation.
    /// </returns>
    Task<HttpStatusCode> SaveFormAsync(FormBase form);
}
