using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Portal.Web.Components.Controllers;

/// <summary>
/// Abstract base controller for handling form operations with caching and validation support.
/// </summary>
/// <typeparam name="TForm">The type of the form model, which must inherit from <see cref="FormBase"/> and have a parameterless constructor.</typeparam>
public abstract class BaseFormController<TForm> : BaseFormControllerCore where TForm : FormBase, new()
{
    private readonly IFormCacheClient _formCache;
    private TForm _form;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFormController{TForm}"/> class.
    /// </summary>
    /// <param name="formCache">The form cache client used for persisting form data.</param>
    protected BaseFormController(IFormCacheClient formCache)
    {
        _formCache = formCache;
    }

    /// <summary>
    /// Marks the current form as complete and updates the cache.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task SetFormAsCompleteAsync()
    {
        _form = await _formCache.GetFormFromCacheAsync<TForm>();
        _form.IsComplete = true;
        await _formCache.SaveFormToCacheAsync(_form);
    }

    /// <summary>
    /// Returns a view with the persisted form model from the cache.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the form model.
    /// </returns>
    protected override async Task<IActionResult> ViewWithPersistedModelAsync()
    {
        _form = await _formCache.GetFormFromCacheAsync<TForm>();
        return View(_form);
    }

    /// <summary>
    /// Validates a specific property of the form, updates its value, and redirects to the next action if validation succeeds.
    /// If the model state is invalid, returns the view with the current form model for user correction.
    /// </summary>
    /// <param name="property">The name of the property to validate and update.</param>
    /// <param name="value">The value to set for the specified property.</param>
    /// <param name="nextAction">The name of the next action to redirect to if validation succeeds.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that either returns the view with the form model if validation fails,
    /// or redirects to the next action if validation succeeds.
    /// </returns>
    protected override async Task<IActionResult> ValidateAndRedirectToNextSectionAsync(string property, object? value, string nextAction)
    {
        _form = await _formCache.GetFormFromCacheAsync<TForm>();
        SetPropertyValueByName(_form, property, value);

        return ValidateAndRedirectToNextSection(_form, nextAction);
    }

    /// <summary>
    /// Validates the provided form model and, if valid, persists it to the cache and redirects to the specified next action.
    /// If the model state is invalid, returns the view with the current model for user correction.
    /// </summary>
    /// <param name="model">The form model to validate and persist.</param>
    /// <param name="nextAction">The name of the next action to redirect to if validation succeeds.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that either returns the view with the model if validation fails,
    /// or redirects to the next action if validation succeeds.
    /// </returns>
    protected async Task<IActionResult> ValidateAndRedirectToNextSectionAsync(TForm model, string nextAction)
    {
        return ValidateAndRedirectToNextSection(model, nextAction);
    }

    /// <summary>
    /// Validates the provided form model. If the model state is invalid, returns the view with the current form model for user correction.
    /// If the model state is valid, persists the form to the cache and redirects to the specified next action.
    /// </summary>
    /// <param name="form">The form model to validate and persist.</param>
    /// <param name="nextAction">The name of the next action to redirect to if validation succeeds.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that either returns the view with the form model if validation fails,
    /// or redirects to the next action if validation succeeds.
    /// </returns>
    private IActionResult ValidateAndRedirectToNextSection(TForm form, string nextAction)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        _formCache.SaveFormToCacheAsync(form);

        return RedirectToAction(nextAction);
    }
}