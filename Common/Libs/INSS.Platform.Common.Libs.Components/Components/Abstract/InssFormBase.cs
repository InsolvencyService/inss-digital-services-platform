using INSS.Platform.Common.Libs.Components.Common;
using INSS.Platform.Common.Libs.Components.Components.Abstract;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

/// <summary>
/// Provides a base Blazor component for form handling, including session management, navigation, and form metadata.
/// </summary>
/// <typeparam name="TForm">The type of form model, which must inherit from <see cref="FormBase"/> and have a parameterless constructor.</typeparam>
public abstract class InssFormBase<TForm> : InssCommonBase where TForm : FormBase, new()
{
    /// <summary>
    /// Gets or sets the Blazor <see cref="NavigationManager"/> for page navigation.
    /// </summary>
    [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the application configuration service.
    /// </summary>
    [Inject] protected IConfiguration Configuration { get; set; } = default!;

    /// <summary>
    /// Gets or sets the service for retrieving form instance metadata.
    /// </summary>
    [Inject] protected IFormMetadataService FormMetadataService { get; set; } = default!;

    /// <summary>
    /// Gets or sets the API client for posting form data.
    /// </summary>
    [Inject] protected IFormApiClient ApiClient { get; set; } = default!;

    [Inject] protected IPropertyValidator PropertyValidator { get; set; } = default!;

    /// <summary>
    /// The name of the main form used for parameter binding.
    /// </summary>
    protected const string FormName = "main-form";

    /// <summary>
    /// Gets or sets the <see cref="EditContext"/> for the current form, used for validation and editing.
    /// </summary>
    protected EditContext CurrentEditContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the form model bound to the main form.
    /// </summary>
    [SupplyParameterFromForm(FormName = FormName)]
    protected TForm Form { get; set; } = new();

    /// <summary>
    /// Initializes the form, loading from session and handling metadata from the query string.
    /// </summary>
    protected void InitializeFormInstance()
    {
        var cachedForm = RetrieveFormFromCache();

        var formMetadata = FormMetadataService.CreateFromQueryString();

        // If metadata is present in the query, treat this as the first call from the launcher.
        if (formMetadata is not null)
        {
            //ListIndex = 0;
            //PageIndex = 0;

            // If this is a different form set instance, start a new form.
            if (cachedForm.FormMetadata?.FormInstanceId != formMetadata.FormInstanceId)
            {
                cachedForm = new TForm();
            }

            // Always update metadata from the query on first call.
            cachedForm.FormMetadata = formMetadata;
            WriteFormToCache(cachedForm);

            // Remove query parameters, 
            // we do not want to keep reading them with every page load 
            // and it is also useful to indicate the very first time the form instance is created.
            var uri = NavigationManager.Uri;
            var baseUri = uri.Split('?')[0];
            NavigationManager.NavigateTo(baseUri, forceLoad: false, replace: true);
        }

        ValidateMetadata(cachedForm);

        Form = cachedForm;
    }

    /// <summary>
    /// Retrieves the current form state from cache.
    /// <para>
    /// <b>Note:</b> The form state is currently persisted in the user's session. 
    /// This implementation may be updated in the future to use a different caching solution, 
    /// such as Redis or distributed cache, to support scalability or multi-server scenarios.
    /// </para>
    /// </summary>
    /// <returns>
    /// The form model of type <typeparamref name="TForm"/> retrieved from session, or a new instance if not found.
    /// </returns>
    protected TForm RetrieveFormFromCache()
    {
        var json = HttpContextAccessor.HttpContext!.Session.GetString(Config.SessionKey);

        var form = string.IsNullOrEmpty(json) ? new TForm() : JsonSerializer.Deserialize<TForm>(json)!;

        return form;
    }

    /// <summary>
    /// Caches the current form state for later retrieval.
    /// <para>
    /// <b>Note:</b> The form is currently persisted to the user's session. 
    /// This implementation may be updated in the future to use a different caching solution, 
    /// such as Redis or distributed cache, to support scalability or multi-server scenarios.
    /// </para>
    /// </summary>
    /// <param name="form">The form model to cache.</param>
    protected void WriteFormToCache(TForm form)
    {
        HttpContextAccessor.HttpContext!.Session.SetString(Config.SessionKey, JsonSerializer.Serialize(form));
    }

    /// <summary>
    /// Asynchronously saves the specified form model to the database using the configured API client.
    /// </summary>
    /// <param name="form">
    /// The form model to be saved. This should be an instance of <typeparamref name="TForm"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <c>true</c> if the form was successfully saved to the database; otherwise, <c>false</c>.
    /// </returns>
    protected async Task<bool> SaveFormToDatabase(TForm form)
    {
        return await ApiClient.PostFormDataAsync(form, Configuration["FormsApi-Url"]!);
    }

    /// <summary>
    /// Validates the specified properties of a model and updates the <see cref="ValidationMessageStore"/> for the given <see cref="EditContext"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to validate.</typeparam>
    /// <param name="editContext">The <see cref="EditContext"/> associated with the form.</param>
    /// <param name="model">The model instance to validate.</param>
    /// <param name="properties">An array of property names to validate.</param>
    /// <returns>
    /// <c>true</c> if the model is valid for the specified properties; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsValid<TModel>(EditContext editContext, TModel model, string[] properties)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var errors = PropertyValidator.ValidateProperties(model, properties);
        if (errors.Any())
        {
            ValidationMessageStore validationMessageStore = new ValidationMessageStore(editContext);

            foreach (var error in errors)
            {
                var fieldIdentifier = new FieldIdentifier(model, error.MemberNames.First());
                validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
            }

            editContext.NotifyValidationStateChanged();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates a single property of the form and, if valid, assigns its value to the persisted form instance.
    /// Updates the <see cref="ValidationMessageStore"/> for the current <see cref="EditContext"/>.
    /// </summary>
    /// <param name="formToValidate">The form instance containing the property to validate.</param>
    /// <param name="formToPersist">The form instance to which the property value will be assigned if valid.</param>
    /// <param name="property">The name of the property to validate and assign.</param>
    /// <returns>
    /// <c>true</c> if the property is valid and assigned; otherwise, <c>false</c>.
    /// </returns>
    protected bool ValidateAndAssignFormProperty(TForm formToValidate, TForm formToPersist, string property)
    {
        var value = formToValidate.GetType().GetProperty(property)?.GetValue(formToValidate);

        var errors = PropertyValidator.ValidateProperties(formToValidate, new[] { property });
        if (errors.Any())
        {
            var validationMessageStore = new ValidationMessageStore(CurrentEditContext);
            foreach (var error in errors)
            {
                var fieldIdentifier = new FieldIdentifier(Form, error.MemberNames.First());
                validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
            }

            // Show the validation message.
            CurrentEditContext.NotifyValidationStateChanged();

            // Ensure the invalid value remains in the form for user correction.
            SetPropertyValueByName(Form, property, value);

            return false;
        }

        // Set the valid value to the item to persist
        SetPropertyValueByName(formToPersist, property, value);

        return true;
    }

    /// <summary>
    /// Validates all properties of a complex property and, if valid, assigns it to the persisted form instance.
    /// Updates the <see cref="ValidationMessageStore"/> for the current <see cref="EditContext"/>.
    /// </summary>
    /// <typeparam name="TComplex">The type of the complex property to validate.</typeparam>
    /// <param name="complexPropertyToValidate">The complex property instance to validate.</param>
    /// <param name="formToPersist">The form instance to which the complex property will be assigned if valid.</param>
    /// <param name="propertyName">The name of the property on the form to assign.</param>
    /// <returns>
    /// <c>true</c> if the complex property is valid and assigned; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// Thrown if the property to validate does not exist on the bound instance of the <see cref="Form"/> property.
    /// </exception>
    protected bool ValidateAndAssignFormProperty<TComplex>(
        TComplex complexPropertyToValidate,
        TForm formToPersist,
        string propertyName)
    {
        var errors = PropertyValidator.ValidateAllProperties(complexPropertyToValidate);
        if (errors.Any())
        {
            var validationMessageStore = new ValidationMessageStore(CurrentEditContext);

            var formInstance = typeof(TForm).GetProperty(propertyName)?.GetValue(Form);
            if (formInstance is null)
            {
                throw new NullReferenceException("The property to validate must exist on the bound instance of the Form property.");
            }

            foreach (var error in errors)
            {
                foreach (var memberName in error.MemberNames)
                {
                    var fieldIdentifier = new FieldIdentifier(formInstance, memberName);
                    validationMessageStore.Add(fieldIdentifier, error.ErrorMessage!);
                }
            }

            // Show the validation message.
            CurrentEditContext.NotifyValidationStateChanged();

            /// Copies all property values from <paramref name="complexPropertyToValidate"/> to the corresponding properties
            /// of <paramref name="formInstance"/>. This is used to ensure the invalid values remain in the form for user correction.
            foreach (var prop in typeof(TComplex).GetProperties())
            {
                var value = prop.GetValue(complexPropertyToValidate);
                prop.SetValue(formInstance, value);
            }

            return false;
        }

        /// Copies all property values from <paramref name="complexPropertyToValidate"/> to the corresponding properties
        /// of the target instance on <paramref name="formToPersist"/>. This is used to ensure the valid values are assigned
        /// to the persisted form instance after successful validation.
        var targetInstance = typeof(TForm).GetProperty(propertyName)?.GetValue(formToPersist);
        foreach (var prop in typeof(TComplex).GetProperties())
        {
            var value = prop.GetValue(complexPropertyToValidate);
            prop.SetValue(targetInstance, value);
        }

        return true;
    }

    /// <summary>
    /// Returns the full field name for a property of the <see cref="Form"/>.
    /// </summary>
    /// <param name="propertyName">The name of the property for which to generate the field name.</param>
    /// <returns>
    /// A string representing the full field name in the format "Form.PropertyName".
    /// </returns>
    protected override string FieldName(string propertyName)
    {
        return string.Concat(nameof(Form), ".", propertyName);
    }

    /// <summary>
    /// Sets the value of a property on the specified target object by property name.
    /// </summary>
    /// <param name="target">
    /// The object whose property value will be set.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property to set.
    /// </param>
    /// <param name="value">
    /// The value to assign to the property.
    /// </param>
    protected void SetPropertyValueByName(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }

    /// <summary>
    /// Validates that the form metadata is present; navigates to the error page if not.
    /// </summary>
    /// <param name="form">The form model to validate.</param>
    private void ValidateMetadata(TForm form)
    {
        if (form?.FormMetadata is null)
        {
            NavigationManager.NavigateTo("/Error");
        }
    }
}