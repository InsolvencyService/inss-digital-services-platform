using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.AlphaDemo.Web.Factories;

/// <summary>
/// Provides a factory for creating and initializing instances of <see cref="FormModel"/> for the web application.
/// </summary>
/// <remarks>
/// This implementation creates a new <see cref="FormModel"/> and invokes <see cref="FormModel.Initialize"/> to ensure
/// the model is in a valid, ready-to-use state for Razor Pages.
/// </remarks>
public sealed class WebAppFormModelFactory : IFormModelFactory
{
    /// <summary>
    /// Asynchronously creates a new, initialized instance of <see cref="FormModel"/>.
    /// </summary>
    /// <returns>
    /// A task that, when completed, contains the initialized <see cref="FormModel"/>.
    /// </returns>
    /// <remarks>
    /// The returned task is completed synchronously using <see cref="Task.FromResult{TResult}(TResult)"/>.
    /// </remarks>
    public Task<FormModel> CreateAsync()
    {
        FormModel form = new();

        form.Initialize();

        return Task.FromResult(form);
    }
}