using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public sealed class FormSerializationService : IFormSerializationService
{
    private readonly JsonSerializerOptions _options;
    
    public FormSerializationService(IModelTypeService modelTypeService)
    {
        _options = CreateOptions(modelTypeService);
    }
    
    public string Serialize(FormModel model)
    {
        return JsonSerializer.Serialize(model, _options);
    }

    public FormModel Deserialize(string json)
    {
        return JsonSerializer.Deserialize<FormModel>(json, _options) 
               ?? throw new InvalidOperationException("Unable to deserialize the json to a form model.");
    }
    
    private static JsonSerializerOptions CreateOptions(IModelTypeService modelTypeService)
    {
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    typeInfo =>
                    {
                        if (typeInfo.Type == typeof(BaseModel))
                        {
                            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = "$type"
                            };

                            foreach (JsonDerivedType type in GetJsonDerivedTypes(modelTypeService))
                            {
                                typeInfo.PolymorphismOptions.DerivedTypes.Add(type);
                            }
                        }
                    }
                }
            }
        };

        return options;
    }
    
    private static List<JsonDerivedType> GetJsonDerivedTypes(IModelTypeService modelTypeService)
    {
        List<JsonDerivedType> derivedPageModelTypes = [];

        foreach (Type page in modelTypeService.GetModelTypes())
        {
            derivedPageModelTypes.Add(new JsonDerivedType(page, page.Name));
        }

        return derivedPageModelTypes;
    }
}