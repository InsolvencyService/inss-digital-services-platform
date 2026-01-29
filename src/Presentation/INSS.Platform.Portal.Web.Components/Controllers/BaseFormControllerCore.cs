using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace INSS.Platform.Portal.Web.Components.Controllers;

/// <summary>
/// Provides a base controller for form handling, including model persistence, validation, and property manipulation.
/// </summary>
public abstract class BaseFormControllerCore : Controller
{
    /// <summary>
    /// Returns a view with the persisted model asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
    /// </returns>
    protected abstract Task<IActionResult> ViewWithPersistedModelAsync();

    /// <summary>
    /// Validates the specified property and value, then redirects to the next section asynchronously.
    /// </summary>
    /// <param name="property">The name of the property to validate.</param>
    /// <param name="value">The value to validate.</param>
    /// <param name="nextAction">The action to redirect to if validation succeeds.</param>
    /// <returns>
    /// A <see cref="Task{IActionResult}"/> representing the asynchronous operation.
    /// </returns>
    protected abstract Task<IActionResult> ValidateAndRedirectToNextSectionAsync(string property, object? value, string nextAction);

    /// <summary>
    /// Marks the form or form list as complete asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    protected abstract Task SetFormAsCompleteAsync();

    /// <summary>
    /// Sets the value of a property on the target object by property name.
    /// </summary>
    /// <param name="target">The object whose property value will be set.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    protected static void SetPropertyValueByName(object target, string propertyName, object? value)
    {
        PropertyInfo? property = target.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }
}

