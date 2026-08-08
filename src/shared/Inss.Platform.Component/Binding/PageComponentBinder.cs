using System.ComponentModel;
using System.Reflection;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Inss.Platform.Component.Binding;

public sealed class PageComponentBinder : IModelBinder
{
    private static readonly Dictionary<string, Type> _componentTypes = GetComponentTypes();
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        IFormCollection form = bindingContext.HttpContext.Request.Form;
        PageModel page = new() { Title = form["Title"]!, Path = new PagePath(form["Path.Value"]!), QueryParams = form["QueryParams"] };
        int startIndex = FindStartIndex(form);

        if (startIndex == -1)
        {
            return Task.CompletedTask;
        }
        
        for (int index = startIndex; index < 100; index++)
        {
            KeyValuePair<string, StringValues>[] entries = form.Where(f => f.Key.StartsWith($"Components[{index}].", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (entries.Length == 0)
            {
                // Done
                break;
            }

            // TODO: Make more robust
            string componentTypeName = form[$"Components[{index}].TypeName"]!;
            Type componentType = ResolveComponentType(componentTypeName);
            ComponentModel componentInstance = (ComponentModel)Activator.CreateInstance(componentType)!;

            const BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
            
            foreach (KeyValuePair<string, StringValues> entry in entries)
            {
                if (entry.Key == $"Components[{index}].TypeName")
                {
                    // Skip
                    continue;
                }

                PropertyInfo? property;
                object? convertedValue;
                
                if (entry.Key == $"Components[{index}].Id.Value")
                {
                    // TODO: Make safe version
                    property = componentType.GetProperty("Id", propertyFlags)!;
                    convertedValue = ConvertValue(entry.Value.ToString(), property.PropertyType);
                    property.SetValue(componentInstance, convertedValue);
                    continue;
                }
                
                string propertyName = entry.Key.Replace($"Components[{index}].", string.Empty);
                property = componentType.GetProperty(propertyName, propertyFlags);

                if (property is null || !property.CanWrite)
                {
                    continue;
                }
                
                convertedValue = ConvertValue(entry.Value.ToString(), property.PropertyType);
                property.SetValue(componentInstance, convertedValue);
            }

            page.Components = [..page.Components, componentInstance];
        }
        
        bindingContext.Result = ModelBindingResult.Success(page);
        return Task.CompletedTask;
    }

    private static int FindStartIndex(IFormCollection form)
    {
        for (int index = 0; index < 100; index++)
        {
            KeyValuePair<string, StringValues>[] entries = 
                form.Where(f => f.Key.StartsWith($"Components[{index}].", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (entries.Length > 0)
            {
                return index;
            }
        }

        return -1;
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

        TypeConverter converter = TypeDescriptor.GetConverter(underlyingType);
        return converter.ConvertFromString(value);
        //return Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
    }
    
    private static Type ResolveComponentType(string typeName)
    {
        return !_componentTypes.TryGetValue(typeName, out Type? value) 
            ? throw new ComponentException($"Unable to find the full type for {typeName}.") 
            : value;
    }
    
    private static Assembly[] GetAll()
    {
        List<Assembly> modelAssemblies = [typeof(ComponentModel).Assembly];
                
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                     a.FullName?.StartsWith("Inss.", StringComparison.OrdinalIgnoreCase) == true))
        {
            modelAssemblies.Add(assembly);
        }
        
        return  modelAssemblies.ToArray();
    }

    private static Dictionary<string, Type> GetComponentTypes()
    {
        Assembly[] assemblies = GetAll();
        Type contentModelType = typeof(ComponentModel);
        Dictionary<string, Type> componentTypeList = [];
        
        foreach (Type type in GetAllTypes(assemblies))
        {
            if (!componentTypeList.ContainsKey(type.FullName!) && 
                type != contentModelType && 
                !type.IsAbstract && 
                contentModelType.IsAssignableFrom(type))
            {
                componentTypeList.Add(type.FullName!, type);
            }
        }

        return componentTypeList;
    }
    
    private static IEnumerable<Type> GetAllTypes(Assembly[] assemblies)
    {
        return assemblies.SelectMany(assembly => assembly.GetTypes());
    }
}