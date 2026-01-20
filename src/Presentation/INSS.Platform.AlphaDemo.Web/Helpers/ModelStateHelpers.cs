using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace INSS.Platform.AlphaDemo.Web.Helpers;

/// <summary>
/// Provides helper methods for manipulating and filtering <see cref="ModelStateDictionary"/> entries.
/// </summary>
public static class ModelStateHelpers
{
    /// <summary>
    /// Removes all model state entries except those matching the specified property names.
    /// </summary>
    /// <param name="modelState">The model state dictionary to filter.</param>
    /// <param name="propertyNames">The property names to retain in the model state.</param>
    public static void OnlyValidateProperty(ModelStateDictionary modelState, params string[] propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            return;
        }

        List<string> keysToRemove = [.. modelState.Keys.Where(key => !propertyNames.Any(p => key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))];

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }

    /// <summary>
    /// Removes all model state entries for a collection property except those matching the specified property names at the given index.
    /// </summary>
    /// <param name="modelState">The model state dictionary to filter.</param>
    /// <param name="collectionPropertyName">The name of the collection property.</param>
    /// <param name="collectionIndex">The index of the collection item to validate.</param>
    /// <param name="propertyNames">The property names to retain for the specified collection item.</param>
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

        List<string> keysToRemove = [.. modelState.Keys
            .Where(key => !keysToKeep.Any(keepKey => key.EndsWith(keepKey, StringComparison.OrdinalIgnoreCase)))];

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }

    /// <summary>
    /// Removes all model state entries except those matching the public instance property names of the specified model object.
    /// </summary>
    /// <param name="modelState">The model state dictionary to filter.</param>
    /// <param name="model">The model object whose property names will be retained in the model state.</param>
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

        List<string> keysToRemove = [.. modelState.Keys
            .Where(key => !propertyNames.Any(p => key.EndsWith(p, StringComparison.OrdinalIgnoreCase)))];

        foreach (string key in keysToRemove)
        {
            modelState.Remove(key);
        }
    }
}