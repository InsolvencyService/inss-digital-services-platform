using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Serialization.Converters;

namespace GovUk.Forms.Domain.Serialization;

public static class FormSerializer
{
    private static JsonSerializerOptions? _options;
    private static readonly Type _baseModelType = typeof(PageModel);
    
    public static void Initialize(params Assembly[] assemblies)
    {
        _options ??= CreateOptions(assemblies);
    }
    
    public static string SerializeForm(FormModel form)
    {
        return JsonSerializer.Serialize(form, _options);
    }

    public static FormModel DeserializeForm(string json)
    {
        return JsonSerializer.Deserialize<FormModel>(json, _options)!;
    }
    
    public static string SerializePage(PageModel content)
    {
        return JsonSerializer.Serialize(content, _options);
    }

    public static PageModel DeserializePage(string json, Type type)
    {
        return (PageModel)JsonSerializer.Deserialize(json, type, _options)!
               ?? throw new ModelException($"Unable to deserialize the json to a {type.FullName} model.");
    }
    
    public static string SerializeState<T>(T state)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(state, _options)));
    }
    
    public static T DeserializeState<T>(string state)
    {
        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(Convert.FromBase64String(state)), _options)!;
    }
    
    private static JsonSerializerOptions CreateOptions(params Assembly[] assemblies)
    {
        List<Type> modelTypes = [];
        
        foreach (Type type in assemblies.SelectMany(a => a.GetTypes()))
        {
            if (type != _baseModelType && !type.IsAbstract && _baseModelType.IsAssignableFrom(type))
            {
                modelTypes.Add(type);
            }
        }

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { typeInfo => AddPolymorphicTypeDiscriminators(modelTypes, typeInfo) }
            }
            //WriteIndented = true
        };

        options.Converters.Add(new ContentIdJsonConverter());
        options.Converters.Add(new ContentPathJsonConverter());
        options.Converters.Add(new NodeIdJsonConverter());
        options.Converters.Add(new PageEditTypeJsonConverter());
        options.Converters.Add(new SectionStateTypeJsonConverter());
        options.Converters.Add(new GroupIdJsonConverter());
        options.Converters.Add(new SubmitTypeJsonConverter());
        
        return options;
    }

    private static void AddPolymorphicTypeDiscriminators(List<Type> modelTypes, JsonTypeInfo typeInfo)
    {
        if (typeInfo.Type == _baseModelType)
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions { TypeDiscriminatorPropertyName = "$type" };

            foreach (JsonDerivedType type in GetJsonDerivedTypes(modelTypes))
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
}