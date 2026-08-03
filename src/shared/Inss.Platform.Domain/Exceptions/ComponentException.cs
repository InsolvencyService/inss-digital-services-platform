namespace Inss.Platform.Domain.Exceptions;

public sealed class ComponentException : Exception
{
    public ComponentException(string message) : base(message)
    {
        
    }
}