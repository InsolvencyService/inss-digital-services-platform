using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Reflection;

namespace INSS.Platform.AlphaDemo.Web.Services;

public static class ModelStateHelpers
{
    /// <summary>
    /// Removes all ModelState entries except those matching the provided property names.
    /// </summary>
    /// <param name="modelState">The ModelStateDictionary to modify.</param>
    /// <param name="propertyNames">The property names to keep (case-insensitive, matches end of key).</param>
    public static void OnlyValidateProperty(ModelStateDictionary modelState, params string[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            return;
        }

        List<string> keysToRemove = modelState.Keys
            .Where(key => !propertyNames.Any(p => key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }



    public static void OnlyValidateCollectionProperty(ModelStateDictionary modelState, string collectionPropertyName, int collectionIndex, params string[] propertyNames)
    {
        if (string.IsNullOrEmpty(collectionPropertyName) || propertyNames == null || propertyNames.Length == 0)
        {
            return;
        }

        // Example key: "CollectionPropertyName[0].PropertyName"
        HashSet<string> keysToKeep = propertyNames
            .Select(p => $"{collectionPropertyName}[{collectionIndex}].{p}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<string> keysToRemove = modelState.Keys
            .Where(key => !keysToKeep.Any(keepKey => key.EndsWith(keepKey, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }

    public static void SetPropertyValueByName(object target, string propertyName, object? value)
    {
        PropertyInfo? property = target.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }

    public static void SetCollectionPropertyValueByName(object target, string collectionPropertyName, int collectionIndex, string propertyName, object? value)
    {
        PropertyInfo? collectionProperty = target.GetType().GetProperty(collectionPropertyName);
        if (collectionProperty != null && collectionProperty.CanRead)
        {
            if (collectionProperty.GetValue(target) is IList collection && collectionIndex >= 0 && collectionIndex < collection.Count)
            {
                object? item = collection[collectionIndex];
                PropertyInfo? itemProperty = item?.GetType()?.GetProperty(propertyName);
                if (itemProperty != null && itemProperty.CanWrite)
                {
                    itemProperty.SetValue(item, value);
                }
            }
        }
    }

    public static void OnlyValidateObjectProperties(ModelStateDictionary modelState, object model)
    {
        if (model == null)
        {
            return;
        }

        HashSet<string> propertyNames = model.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<string> keysToRemove = modelState.Keys
            .Where(key => !propertyNames.Any(p => key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }

}