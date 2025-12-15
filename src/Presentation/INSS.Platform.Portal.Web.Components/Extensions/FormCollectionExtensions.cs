using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace INSS.Platform.Portal.Web.Components.Extensions;

public static class FormCollectionExtensions
{
    public static Dictionary<string, object?> ToObjectDictionary(this IFormCollection form, Type type)
    {
        Dictionary<string, object?> result = new(StringComparer.OrdinalIgnoreCase);
        
        foreach (KeyValuePair<string, StringValues> entry in form)
        {
            PropertyInfo? property = type.GetProperty(entry.Key);

            if (property is null)
            {
                continue;
            }
            
            result[entry.Key] = ChangeType(entry.Value.ToString(), property.PropertyType);
        }

        return result;
    }
    
    private static object? ChangeType(object? value, Type conversionType)
    {
        Type targetType = Nullable.GetUnderlyingType(conversionType) ?? conversionType;

        if (value is null)
        {
            return conversionType.IsValueType && Nullable.GetUnderlyingType(conversionType) == null
                ? Activator.CreateInstance(conversionType)
                : null;
        }

        if (value is string s && string.IsNullOrWhiteSpace(s))
        {
            if (conversionType == typeof(string))
            {
                return string.Empty;    
            }
            return conversionType.IsValueType && Nullable.GetUnderlyingType(conversionType) == null
                ? Activator.CreateInstance(conversionType)
                : null;
        }

        return Convert.ChangeType(value, targetType, Thread.CurrentThread.CurrentCulture);
    }
}