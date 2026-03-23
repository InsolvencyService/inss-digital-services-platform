using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Components.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ComponentException : Exception
{
    internal ComponentException(string message) : base(message)
    {
    }
}