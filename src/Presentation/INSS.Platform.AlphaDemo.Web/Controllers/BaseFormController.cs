using INSS.Platform.AlphaDemo.Web.Helpers;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

/// <summary>
/// Abstract base controller for handling form operations with caching and validation support.
/// </summary>
/// <typeparam name="TForm">The type of form, must inherit from <see cref="FormBase"/> and have a parameterless constructor.</typeparam>
public abstract class BaseFormController<TForm> : Controller where TForm : FormBase, new()
{
    private readonly IFormCacheClient _formCache;
    private readonly string _cacheKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFormController{TForm}"/> class.
    /// </summary>
    /// <param name="formCache">The form cache client used for persisting form data.</param>
    protected BaseFormController(IFormCacheClient formCache)
    {
        _formCache = formCache;
        _cacheKey = _formCache.GetFormCacheKey<TForm>();
    }

    /// <summary>
    /// Marks the current form as complete and updates the cache.
    /// </summary>
    protected virtual void SetFormAsComplete()
    {
        TForm form = _formCache.GetFormFromCache<TForm>(_cacheKey)!;
        form.IsComplete = true;
        _formCache.SetFormToCache(_cacheKey, form);
    }

    /// <summary>
    /// Returns a view with the persisted form model from the cache.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the form model.</returns>
    protected virtual IActionResult ViewWithPersistedModel()
    {
        TForm form = _formCache.GetFormFromCache<TForm>(_cacheKey)!;
        return View(form);
    }

    /// <summary>
    /// Validates a section of the form and redirects to the next action if valid, otherwise returns the view with the current model.
    /// </summary>
    /// <param name="model">The form model to validate.</param>
    /// <param name="modelState">The model state dictionary for validation.</param>
    /// <param name="property">The property name to validate.</param>
    /// <param name="value">The value to validate and persist.</param>
    /// <param name="nextAction">The name of the next action to redirect to if validation succeeds.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that either returns the view with the model if validation fails,
    /// or redirects to the next action if validation succeeds.
    /// </returns>
    protected virtual async Task<IActionResult> ValidateAndRedirectToNextSectionAsync(
        FormBase model, ModelStateDictionary modelState, string property, object? value, string nextAction)
    {
        if (!ValidateSection(modelState, property, value))
        {
            return View(model);
        }

        TForm form = _formCache.GetFormFromCache<TForm>(_cacheKey)!;
        PropertyHelpers.SetPropertyValueByName(form, property, value);
        _formCache.SetFormToCache(_cacheKey, form);

        return RedirectToAction(nextAction);
    }

    /// <summary>
    /// Validates a specific section or property of the form model.
    /// </summary>
    /// <param name="modelState">The model state dictionary for validation.</param>
    /// <param name="property">The property name to validate.</param>
    /// <param name="value">The value to validate. If complex, validates all properties.</param>
    /// <returns><c>true</c> if the model state is valid; otherwise, <c>false</c>.</returns>
    protected static bool ValidateSection(ModelStateDictionary modelState, string property, object? value)
    {
        if (value != null && TypeHelpers.IsComplexObject(value))
        {
            ModelStateHelpers.OnlyValidateObjectProperties(modelState, value);
        }
        else
        {
            ModelStateHelpers.OnlyValidateProperty(modelState, property);
        }

        return modelState.IsValid;
    }
}