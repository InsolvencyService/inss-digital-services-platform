using System.Reflection;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public sealed class ModelTypeService : IModelTypeService
{
    private readonly List<Type> _derivedModelTypes = [];
    
    public ModelTypeService(params Assembly[] assemblies)
    {
        List<Assembly> fullAssemblyList = [..assemblies, typeof(BaseModel).Assembly];

        Type baseModelType = typeof(BaseModel);

        foreach (Assembly assembly in fullAssemblyList)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type != baseModelType && !type.IsAbstract && baseModelType.IsAssignableFrom(type))
                {
                    _derivedModelTypes.Add(type);
                }
            }
        }
    }
    
    public Type GetModelType(string name)
    {
        Type? type = _derivedModelTypes.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return type ?? throw new InvalidOperationException($"Unable to find model type for {name}.");
    }

    public IEnumerable<Type> GetModelTypes()
    {
        return this._derivedModelTypes.AsReadOnly();
    }
}