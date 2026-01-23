using INSS.Platform.AlphaDemo.Web.Helpers;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Enums;
using INSS.Platform.Portal.Domain.Forms;
using INSS.Platform.Shared.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

/// <summary>
/// Abstract base controller for managing a list of form items.
/// Provides common list operations such as add, remove, change, and summary.
/// </summary>
/// <typeparam name="TFormItem">The type of form item, must inherit from <see cref="FormBase"/> and have a parameterless constructor.</typeparam>
public abstract class BaseFormListController<TFormItem> : BaseFormController<TFormItem> where TFormItem : FormBase, new()
{
    private readonly IFormCacheClient _formCache;
    private readonly string _cacheKey;
    private readonly string _itemName;
    private readonly string _itemListTextPropertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFormListController{TFormItem}"/> class.
    /// </summary>
    /// <param name="formCache">The form cache client.</param>
    /// <param name="itemName">The display name for the item type.</param>
    /// <param name="listTextPropertyName">The property name to use for displaying items in the list.</param>
    protected BaseFormListController(IFormCacheClient formCache, string itemName, string listTextPropertyName)
        : base(formCache)
    {
        _formCache = formCache;
        _cacheKey = _formCache.GetFormCacheKey<TFormItem>();
        _itemName = itemName;
        _itemListTextPropertyName = listTextPropertyName;
    }

    /// <summary>
    /// Called before an action executes, ensures at least one item exists in the list.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
        if (formItems.Count == 0)
        {
            formItems.Add(new TFormItem());
            _formCache.SetFormListToCache(_cacheKey, formItems);
            _formCache.SetCurrentFormListIndex(_cacheKey, 0);
        }
    }

    /// <summary>
    /// Displays the list of items.
    /// </summary>
    /// <returns>The list view with the populated model.</returns>
    public IActionResult List()
    {
        return View(model: PopulateListModel());
    }

    /// <summary>
    /// Displays a summary of the list items.
    /// </summary>
    /// <returns>The summary view with the list of form items.</returns>
    public IActionResult ListSummary()
    {
        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);

        return View(model: formItems);
    }

    /// <summary>
    /// Handles the addition of a new item to the list.
    /// </summary>
    /// <param name="model">The list model containing confirmation and other data.</param>
    /// <returns>Redirects or returns the list view depending on model state and confirmation.</returns>
    [HttpPost]
    public IActionResult Add(ListModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(List), PopulateListModel());
        }

        if(model.Confirm == Confirmation.Yes)
        {
            List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
            formItems.Add(new TFormItem());
            _formCache.SetFormListToCache(_cacheKey, formItems);
            _formCache.SetCurrentFormListIndex(_cacheKey, formItems.Count - 1);

            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(ListSummary));
    }

    /// <summary>
    /// Changes the current item index and optionally redirects to a specific field action.
    /// </summary>
    /// <param name="itemIndex">The index of the item to change.</param>
    /// <param name="fieldName">Optional field name to redirect to.</param>
    /// <returns>Redirects to the specified action.</returns>
    [Route("Change/{itemIndex}/{fieldName?}")]
    public IActionResult Change(int itemIndex, string? fieldName = null)
    {
        _formCache.SetCurrentFormListIndex(_cacheKey, itemIndex);

        return RedirectToAction(string.IsNullOrWhiteSpace(fieldName) ? "Index" : fieldName);
    }

    /// <summary>
    /// Removes an item from the list at the specified index.
    /// </summary>
    /// <param name="itemIndex">The index of the item to remove.</param>
    /// <returns>Returns the list view or redirects to the index if the list is empty.</returns>
    [Route("Remove/{itemIndex}")]
    public IActionResult Remove(int itemIndex)
    {
        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
        if(formItems.Count > 0)
        {
            formItems.RemoveAt(itemIndex);

            _formCache.SetFormListToCache(_cacheKey, formItems);
        }

        if(formItems.Count > 0)
        {
            _formCache.SetCurrentFormListIndex(_cacheKey, formItems.Count-1);

            return View(nameof(List), PopulateListModel());
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays a confirmation view for removing an item.
    /// </summary>
    /// <param name="itemIndex">The index of the item to confirm removal for.</param>
    /// <returns>The confirmation view model.</returns>
    [Route("ConfirmRemove/{itemIndex}")]
    public IActionResult ConfirmRemove(int itemIndex)
    {
        ListModel listModel = PopulateListModel();

        ListConfirmationModel model = new ()
        {
            Question = $"Are you sure you want to remove {listModel.Items[itemIndex]}?",
            Confirm = null,
            ItemIndex = itemIndex
        };

        return View(model);
    }

    /// <summary>
    /// Handles the confirmation of item removal.
    /// </summary>
    /// <param name="model">The confirmation model.</param>
    /// <returns>Redirects to remove or list view based on confirmation.</returns>
    [HttpPost]
    public IActionResult ConfirmRemoveItem(ListConfirmationModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(ConfirmRemove), model);
        }

        if(model.Confirm == Confirmation.Yes)
        {
            return RedirectToAction(nameof(Remove), new { model.ItemIndex });
        }

        return RedirectToAction(nameof(List));
    }

    /// <summary>
    /// Returns the view with the currently persisted model item.
    /// </summary>
    /// <returns>The view for the current form item.</returns>
    protected override IActionResult ViewWithPersistedModel()
    {
        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
        int itemIndex = _formCache.GetCurrentFormListIndex(_cacheKey);

        return View(formItems[itemIndex]);
    }

    /// <summary>
    /// Validates the model and redirects to the next section if valid.
    /// </summary>
    /// <param name="model">The form model.</param>
    /// <param name="modelState">The model state dictionary.</param>
    /// <param name="property">The property being validated.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="nextAction">The next action to redirect to.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the action result.</returns>
    protected override async Task<IActionResult> ValidateAndRedirectToNextSectionAsync(
        FormBase model, ModelStateDictionary modelState, string property, object? value, string nextAction)
    {
        if (!ValidateSection(modelState, property, value))
        {
            return View(model);
        }

        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
        int itemIndex = _formCache.GetCurrentFormListIndex(_cacheKey);

        PropertyHelpers.SetPropertyValueByName(formItems[itemIndex], property, value);
        _formCache.SetFormListToCache(_cacheKey, formItems);

        return RedirectToAction(nextAction);
    }

    /// <summary>
    /// Marks all items in the list as complete.
    /// </summary>
    protected override void SetFormAsComplete()
    {
        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);
        foreach (TFormItem item in formItems)
        {
            item.IsComplete = true;
        }
        _formCache.SetFormListToCache(_cacheKey, formItems);
    }

    /// <summary>
    /// Populates the list model for display in the list view.
    /// </summary>
    /// <returns>A <see cref="ListModel"/> containing item names and display values.</returns>
    private ListModel PopulateListModel()
    {
        List<TFormItem> formItems = _formCache.GetFormListFromCache<TFormItem>(_cacheKey);

        return new ListModel
        {
            ItemName = _itemName,
            Items = GetPropertyValuesAsStrings(formItems, typeof(TFormItem), _itemListTextPropertyName)
        };
    }

    /// <summary>
    /// Retrieves the string representations of a specified property for each item in the form items list.
    /// Handles properties that may implement <see cref="IHasValue{T}"/> and if so returns the Value property.
    /// Also handles enums, returning their descriptions if available.
    /// </summary>
    /// <param name="formItems">The list of form items to extract property values from.</param>
    /// <param name="derivedType">The type of the form item, used to reflect the property.</param>
    /// <param name="propertyName">The name of the property to extract from each item.</param>
    /// <returns>A list of string representations of the specified property for each form item.</returns>
    private static List<string> GetPropertyValuesAsStrings(List<TFormItem> formItems, Type derivedType, string propertyName)
    {
        return [.. formItems
            .Select(item =>
            {
                PropertyInfo? property = derivedType.GetProperty(propertyName);
                object? value = property?.GetValue(item);

                if (value != null)
                {
                    Type? hasValueInterface = value.GetType()
                        .GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHasValue<>));

                    if (hasValueInterface != null)
                    {
                        PropertyInfo? valueProperty = hasValueInterface.GetProperty("Value");
                        object? innerValue = valueProperty?.GetValue(value);

                        if (innerValue is Enum enumInner)
                        {
                            return enumInner.Description();
                        }

                        return innerValue?.ToString() ?? string.Empty;
                    }

                    if (value is Enum enumValue)
                    {
                        return enumValue.Description();
                    }
                }

                return value?.ToString() ?? string.Empty;
            })];
    }
}