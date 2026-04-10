using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Infrastructure.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class UnauthenticatedUserException : Exception
{
    internal UnauthenticatedUserException(string message) : base(message)
    {
    }
}