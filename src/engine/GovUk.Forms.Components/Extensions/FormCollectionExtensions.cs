using System.Globalization;
using System.Reflection;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace GovUk.Forms.Components.Extensions;

public static class FormCollectionExtensions
{
    extension(IFormCollection formCollection)
    {
        public ContentModel HydrateContentModel(ITypeNameResolver typeNameResolver)
        {
            string typeName = formCollection["TypeName"].ToString();
            Type targetType = typeNameResolver.Resolve(typeName);
            ContentModel contentModel = (ContentModel)Activator.CreateInstance(targetType)!;
            BindModel(contentModel, formCollection);
            return contentModel;
        }
    }
    
    private static void BindModel<T>(T target, IFormCollection formCollection, char separator = '.') where T : notnull
    {
        const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        Dictionary<string, StringValues> formDictionary = MergeFormAndFileData(formCollection);
            
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
    }

    private static Dictionary<string, StringValues> MergeFormAndFileData(IFormCollection formCollection)
    {
        Dictionary<string, StringValues> formDictionary = formCollection.ToDictionary();
        IFormFileCollection files = formCollection.Files;

        if (files.Count > 1)
        {
            throw new NotSupportedException("The form engine does not support multiple files being uploaded.");
        }

        if (files.Count == 1)
        {
            using var memoryStream = new MemoryStream();
            files[0].CopyTo(memoryStream);
            formDictionary.Add("Filename", new StringValues(files[0].FileName));
            formDictionary.Add("Length", new StringValues(files[0].Length.ToString(CultureInfo.CurrentCulture)));
            formDictionary.Add("Contents", new StringValues(Convert.ToBase64String(memoryStream.ToArray())));
        }

        return formDictionary;
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

        return Convert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture);
    }
}