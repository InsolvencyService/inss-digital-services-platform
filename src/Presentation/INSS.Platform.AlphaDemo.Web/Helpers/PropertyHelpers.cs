using System.Collections;
using System.Reflection;

namespace INSS.Platform.AlphaDemo.Web.Helpers;

/// <summary>
/// Provides helper methods for setting property values on objects and collections using reflection.
/// </summary>
public static class PropertyHelpers
{
    /// <summary>
    /// Sets the value of a property on the specified target object by property name.
    /// </summary>
    /// <param name="target">The object whose property value will be set.</param>
    /// <param name="propertyName">The name of the property to set.</param>
    /// <param name="value">The value to assign to the property.</param>
    public static void SetPropertyValueByName(object target, string propertyName, object? value)
    {
        PropertyInfo? property = target.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
        }
    }

    /// <summary>
    /// Sets the value of a property on an item within a collection property of the specified target object.
    /// </summary>
    /// <param name="target">The object containing the collection property.</param>
    /// <param name="collectionPropertyName">The name of the collection property.</param>
    /// <param name="collectionIndex">The index of the item within the collection.</param>
    /// <param name="propertyName">The name of the property to set on the collection item.</param>
    /// <param name="value">The value to assign to the property.</param>
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
}