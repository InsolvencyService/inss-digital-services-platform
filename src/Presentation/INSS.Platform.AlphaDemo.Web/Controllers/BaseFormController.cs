using INSS.Platform.AlphaDemo.Web.Helpers;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

/// <summary>
/// Provides a base class for form-based Razor Page models, enabling session management, form data handling, and
/// integration with form metadata and API services.
/// </summary>
/// <remarks>This class is designed to simplify the implementation of form-based workflows in Razor Pages by
/// providing common functionality such as session-based form state management, metadata handling, and API integration.
/// Derived classes can use the provided methods and properties to manage form data and implement custom
/// behavior.</remarks>
/// <typeparam name="TForm">The type of the form associated with the page model. Must inherit from <see cref="FormBase"/> and have a
/// parameterless constructor.</typeparam>
public abstract class BaseFormController<TForm> : Controller where TForm : FormBase, new()
{
    /// <summary>
    /// Gets or sets the session key used to identify the current session.
    /// </summary>
    protected string SessionKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the form data associated with the current request.
    /// </summary>
    public required TForm Form { get; set; }

    /// <summary>
    /// Loads the form data from the current session and deserializes it into the <see cref="Form"/> property.
    /// </summary>
    /// <remarks>
    /// If no session data is found or the session data is empty, a new instance of <typeparamref
    /// name="TForm"/> is created and assigned to the <see cref="Form"/> property.
    /// </remarks>
    protected void LoadFormFromSession()
    {
        Form = FormSessionHelper.LoadFormFromSession<TForm>(HttpContext, SessionKey)!;
    }

    /// <summary>
    /// Saves the specified form to the current session.
    /// </summary>
    /// <remarks>The form is serialized to JSON and stored in the session using a predefined session key.
    /// </remarks>
    /// <param name="form">The form object to be saved.</param>
    protected void SaveFormToSession(TForm form)
    {
        Form = form;
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(Form));
    }

    protected void FormIsComplete()
    {
        LoadFormFromSession();
        Form.IsComplete = true;
        SaveFormToSession(Form);
    }

    protected IActionResult PopulatedView()
    {
        LoadFormFromSession();
        return View(Form);
    }

    protected async Task<IActionResult> Next(FormBase model, ModelStateDictionary modelState, string property, object? value, string nextAction)
    {
        if (value != null && IsComplexObject(value))
        {
            ModelStateHelpers.OnlyValidateObjectProperties(modelState, value);
        }
        else
        {
            ModelStateHelpers.OnlyValidateProperty(modelState, property);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LoadFormFromSession();

        ModelStateHelpers.SetPropertyValueByName(Form, property, value);

        SaveFormToSession(Form);

        return RedirectToAction(nextAction);
    }


    private static bool IsComplexObject(object value)
    {
        Type type = value.GetType();

        // Use TypeCode to efficiently check for String, Decimal, DateTime, Enums, and most Primitives
        if (Type.GetTypeCode(type) != TypeCode.Object)
        {
            return false;
        }

        // Check for remaining simple types (IntPtr/UIntPtr via IsPrimitive, and specific structs/classes)
        return !type.IsPrimitive &&
               type != typeof(Guid) &&
               type != typeof(TimeSpan) &&
               type != typeof(DateTimeOffset) &&
               type != typeof(DateOnly) &&
               type != typeof(TimeOnly) &&
               type != typeof(Uri);
    }
}