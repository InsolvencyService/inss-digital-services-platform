namespace GovUk.Forms.Components.Resolvers;

public interface ITypeNameResolver
{
    Type Resolve(string typeName);
}