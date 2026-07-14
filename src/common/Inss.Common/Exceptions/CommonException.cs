using System.Diagnostics.CodeAnalysis;

namespace Inss.Common.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class CommonException : Exception
{
    internal CommonException(string message) : base(message)
    {
    }
}