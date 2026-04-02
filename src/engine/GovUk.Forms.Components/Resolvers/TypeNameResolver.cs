using System.Reflection;
using GovUk.Forms.Components.Exceptions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Serialization;

namespace GovUk.Forms.Components.Resolvers;

public sealed class TypeNameResolver : ITypeNameResolver
{
    private readonly Dictionary<string, Type> _types = [];
    
    public TypeNameResolver()
    {
        Assembly[] assemblies = ModelAssemblies.GetAll();
        Type contentModelType = typeof(ContentModel);

        foreach (Type type in GetAllTypes(assemblies))
        {
            if (type != contentModelType && !type.IsAbstract && contentModelType.IsAssignableFrom(type))
            {
                _types.Add(type.FullName!, type);
            }
        }
    }
    
    public Type Resolve(string typeName)
    {
        return !_types.TryGetValue(typeName, out Type? value) 
            ? throw new ComponentException($"Unable to find the full type for {typeName}.") 
            : value;
    }

    private static IEnumerable<Type> GetAllTypes(Assembly[] assemblies)
    {
        return assemblies.SelectMany(assembly => assembly.GetTypes());
    }
}