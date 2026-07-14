using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Infrastructure.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class StorageProviderException : Exception
{
    internal StorageProviderException(string message) : base(message)
    {
    }
}