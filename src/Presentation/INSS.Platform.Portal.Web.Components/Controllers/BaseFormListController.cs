using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Domain.Abstract;
using INSS.Platform.Portal.Domain.Enums;
using INSS.Platform.Shared.Web.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace INSS.Platform.Portal.Web.Components.Controllers;

/// <summary>
/// Abstract base controller for managing a list of form items.
/// Provides common actions for listing, adding, changing, and removing items,
/// as well as handling persistence and navigation logic for form lists.
/// </summary>
/// <typeparam name="TFormList">The type representing the form list, must inherit <see cref="FormBase"/> and implement <see cref="IFormListModel{TFormItem}"/>.</typeparam>
/// <typeparam name="TFormItem">The type representing an item in the form list, must inherit <see cref="FormItemBase"/>.</typeparam>
public abstract class BaseFormListController<TFormList, TFormItem> : BaseFormControllerCore
    where TFormList : FormBase, IFormListModel<TFormItem>, new()
    where TFormItem : FormItemBase, new()
{
    private readonly IFormCacheClient _formCache;
    private readonly string _listItemDescription;
    private readonly string _itemListTextPropertyName;
    private TFormList _formList;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFormListController{TFormList, TFormItem}"/> class.
    /// </summary>
    /// <param name="formCache">The form cache client for persisting form data.</param>
    /// <param name="listItemDescription">The display name for the item type in the list.</param>
    /// <param name="listTextPropertyName">The property name to use for displaying items in the list.</param>
    protected BaseFormListController(IFormCacheClient formCache, string listItemDescription, string listTextPropertyName)
    {
        _formCache = formCache;
        _listItemDescription = listItemDescription;
        _itemListTextPropertyName = listTextPropertyName;
    }

    /// <summary>
    /// Called before an action executes, ensures at least one item exists in the list.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The delegate to execute the next action filter or action.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
        if (_formList.Items.Count == 0)
        {
            _formList.Items.Add(new TFormItem());

            await _formCache.SaveFormListToCacheAsync(_formList);
            
            await _formCache.SetCurrentFormListIndexAsync(0);
        }

        await base.OnActionExecutionAsync(context, next);
    }

    /// <summary>
    /// Displays the list of items.
    /// </summary>
    /// <returns>The list view with the populated model.</returns>
    public async Task<IActionResult> List()
    {
        return View(model: await PopulateListModelAsync());
    }

    /// <summary>
    /// Displays a summary of the list items.
    /// </summary>
    /// <returns>The summary view with the list of form items.</returns>
    public async Task<IActionResult> ListSummary()
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();

        return View(model: _formList);
    }

    /// <summary>
    /// Handles the addition of a new item to the list.
    /// </summary>
    /// <param name="model">The list model containing confirmation and other data.</param>
    /// <returns>Redirects or returns the list view depending on model state and confirmation.</returns>
    [HttpPost]
    public async Task<IActionResult> Add(ListModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(List), await PopulateListModelAsync());
        }

        if(model.Confirm == Confirmation.Yes)
        {
            _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
            _formList.Items.Add(new TFormItem());
            
            await _formCache.SaveFormListToCacheAsync(_formList);
            await _formCache.SetCurrentFormListIndexAsync(_formList.Items.Count - 1);

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
    public async Task<IActionResult> Change(int itemIndex, string? fieldName = null)
    {
        await _formCache.SetCurrentFormListIndexAsync(itemIndex);

        return RedirectToAction(string.IsNullOrWhiteSpace(fieldName) ? nameof(Index) : fieldName);
    }

    /// <summary>
    /// Removes an item from the list at the specified index.
    /// </summary>
    /// <param name="itemIndex">The index of the item to remove.</param>
    /// <returns>Returns the list view or redirects to the index if the list is empty.</returns>
    [Route("Remove/{itemIndex}")]
    public async Task<IActionResult> Remove(int itemIndex)
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
        if (_formList.Items.Count > 0 && itemIndex >= 0 && itemIndex < _formList.Items.Count)
        {
            _formList.Items.RemoveAt(itemIndex);
            await _formCache.SaveFormListToCacheAsync(_formList);
        }

        if (_formList.Items.Count > 0)
        {
            await _formCache.SetCurrentFormListIndexAsync(_formList.Items.Count - 1);
            return RedirectToAction(nameof(List));
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays a confirmation view for removing an item.
    /// </summary>
    /// <param name="itemIndex">The index of the item to confirm removal for.</param>
    /// <returns>The confirmation view model.</returns>
    [Route("ConfirmRemove/{itemIndex}")]
    public async Task<IActionResult> ConfirmRemove(int itemIndex)
    {
        ListModel listModel = await PopulateListModelAsync();

        if (itemIndex < 0 || itemIndex >= listModel.Items.Count)
        {
            return RedirectToAction(nameof(List));
        }

        ListConfirmationModel model = new()
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
    /// Returns the view with the currently persisted list item.
    /// </summary>
    /// <returns>The view for the current form item.</returns>
    protected override async Task<IActionResult> ViewWithPersistedModelAsync()
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
        int itemIndex = await _formCache.GetCurrentFormListIndexAsync();

        return View(_formList.Items[itemIndex]);
    }

    /// <summary>
    /// Validates the specified property value for the current form item and redirects to the next section if valid.
    /// Updates the property value, checks model state, persists the updated list, and redirects accordingly.
    /// </summary>
    /// <param name="property">The name of the property to validate and update.</param>
    /// <param name="value">The value to set for the specified property.</param>
    /// <param name="nextAction">The name of the action to redirect to if validation succeeds.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that either returns the view with the current form item if validation fails,
    /// or redirects to the specified next action if validation succeeds.
    /// </returns>
    protected override async Task<IActionResult> ValidateAndRedirectToNextSectionAsync(string property, object? value, string nextAction)
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
        int itemIndex = await _formCache.GetCurrentFormListIndexAsync();

        SetPropertyValueByName(_formList.Items[itemIndex], property, value);

        if (!ModelState.IsValid)
        {
            return View(_formList.Items[itemIndex]);
        }

        await _formCache.SaveFormListToCacheAsync(_formList);

        return RedirectToAction(nextAction);
    }

    /// <summary>
    /// Marks all items in the list as complete.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task SetFormAsCompleteAsync()
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();
        _formList.IsComplete = true;
        await _formCache.SaveFormListToCacheAsync(_formList);
    }

    /// <summary>
    /// Populates the list model for displaying items in the list view.
    /// </summary>
    /// <returns>A <see cref="ListModel"/> containing item names and display values.</returns>
    private async Task<ListModel> PopulateListModelAsync()
    {
        _formList = await _formCache.GetFormListFromCacheAsync<TFormList>();

        return new ListModel
        {
            ItemName = _listItemDescription,
            Items = GetPropertyValuesAsStrings(_formList.Items, typeof(TFormItem), _itemListTextPropertyName)
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