using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ModelException : Exception
{
    internal ModelException(string message) : base(message)
    {
    }
}