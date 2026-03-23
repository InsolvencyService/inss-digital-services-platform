using GovUk.Forms.Components.Exceptions;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Components.Test.Resolvers;

public class TypeNameResolverTests
{
    [Fact]
    public void UnknownType_Resolve_ThrowsException()
    {
        TypeNameResolver resolver = new([typeof(PageModel).Assembly]);
        Type unknownType = typeof(TypeNameResolverTests);

        ComponentException exception = Assert.Throws<ComponentException>(() => resolver.Resolve(unknownType.FullName!));
        
        Assert.Equal($"Unable to find the full type for {unknownType}.", exception.Message);
    }
    
    [Fact]
    public void KnownType_Resolve_ReturnsType()
    {
        TypeNameResolver resolver = new([typeof(PageModel).Assembly]);
        Type expectedType = typeof(AddAnotherModel);
        
        Type type = resolver.Resolve(expectedType.FullName!);
        
        Assert.Equal(expectedType, type);
    }
}