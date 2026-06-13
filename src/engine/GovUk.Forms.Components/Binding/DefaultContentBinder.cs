using System.Globalization;
using System.Reflection;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace GovUk.Forms.Components.Binding;

public class DefaultContentBinder : IContentBinder
{
    private readonly ITypeNameResolver _typeNameResolver;

    public DefaultContentBinder(ITypeNameResolver typeNameResolver)
    {
        _typeNameResolver = typeNameResolver;
    }
    
    public ContentModel BindAndReturnModel(string typeName, IFormCollection formCollection, char separator = '.')
    {
        Type targetType = _typeNameResolver.Resolve(typeName);
        ContentModel target = (ContentModel)Activator.CreateInstance(targetType)!;
        
        const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        Dictionary<string, StringValues> formDictionary = GetFormData(formCollection);
            
        foreach (KeyValuePair<string, StringValues> entry in formDictionary)
        {
            string[] parts = entry.Key.Split(separator);
            object? currentObject = target;
            Type currentType = target.GetType();

            for (int i = 0; i < parts.Length; i++)
            {
                string propertyName = parts[i];
                PropertyInfo? property = currentType.GetProperty(propertyName, propertyFlags);

                if (property is null || !property.CanWrite)
                {
                    continue;
                }

                if (i == parts.Length - 1)
                {
                    // Last part - set value
                    object? convertedValue = ConvertValue(entry.Value.ToString(), property.PropertyType);
                    property.SetValue(currentObject, convertedValue);
                }
                else
                {
                    // Navigate or create nested object
                    object? nestedObject = property.GetValue(currentObject);
                    
                    if (nestedObject is null)
                    {
                        nestedObject = Activator.CreateInstance(property.PropertyType);
                        property.SetValue(currentObject, nestedObject);
                    }

                    currentObject = nestedObject;
                    currentType = currentObject?.GetType() ?? property.PropertyType;
                }
            }
        }

        return target;
    }

    protected virtual Dictionary<string, StringValues> GetFormData(IFormCollection formCollection)
    {
        return formCollection.ToDictionary();
    }
    
    private static object? ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
        {
            return value;
        }

        Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (string.IsNullOrEmpty(value))
        {
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null
                ? Activator.CreateInstance(targetType)
                : null;
        }

        return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
    }
}