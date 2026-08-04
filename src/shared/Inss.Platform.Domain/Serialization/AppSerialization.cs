using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Serialization.Converters;

namespace Inss.Platform.Domain.Serialization;

public static class AppSerialization
{
    private static readonly JsonSerializerOptions? _options;
    private static readonly Type _componentType = typeof(Component);
    
    static AppSerialization()
    {
        Assembly[] assemblies = GetAllTargetAssemblies();
        _options = CreateOptions(assemblies);
    }
    
    public static string Serialize(App appPages)
    {
        return JsonSerializer.Serialize(appPages, _options);
    }
    
    public static App Deserialize(string json)
    {
        return JsonSerializer.Deserialize<App>(json, _options)!;
    }
    
    public static string SerializePage(PageModel page)
    {
        return JsonSerializer.Serialize(page, _options);
    }
    
    public static PageModel DeserializePage(string json)
    {
        return JsonSerializer.Deserialize<PageModel>(json, _options)!;
    }
    
    private static JsonSerializerOptions CreateOptions(params Assembly[] assemblies)
    {
        List<Type> componentTypes = [];
        
        foreach (Type type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (!componentTypes.Contains(type) && type != _componentType && !type.IsAbstract && _componentType.IsAssignableFrom(type))
            {
                componentTypes.Add(type);
            }
        }
        
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { typeInfo => AddPolymorphicTypeDiscriminators(componentTypes, typeInfo) }
            },
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        options.Converters.Add(new PagePathConverter());
        options.Converters.Add(new ComponentIdConverter());
        options.Converters.Add(new ContentConverter());
        options.Converters.Add(new SessionIdConverter());
        options.Converters.Add(new TypeConverter());
        
        return options;
    }
    
    private static void AddPolymorphicTypeDiscriminators(List<Type> componentTypes, JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type == _componentType)
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions { TypeDiscriminatorPropertyName = "$type" };

            foreach (JsonDerivedType type in GetJsonDerivedTypes(componentTypes))
            {
                typeInfo.PolymorphismOptions.DerivedTypes.Add(type);
            }
        }
    }

    private static List<JsonDerivedType> GetJsonDerivedTypes(List<Type> modelTypes)
    {
        List<JsonDerivedType> derivedPageModelTypes = [];
        derivedPageModelTypes.AddRange(modelTypes.Select(type => new JsonDerivedType(type, type.Name)));
        return derivedPageModelTypes;
    }
    
    private static Assembly[] GetAllTargetAssemblies()
    {
        List<Assembly> modelAssemblies = [typeof(PageModel).Assembly];
                
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                     a.FullName?.StartsWith("Inss.", StringComparison.OrdinalIgnoreCase) == true))
        {
            modelAssemblies.Add(assembly);
        }
        
        return  modelAssemblies.ToArray();
    }
}