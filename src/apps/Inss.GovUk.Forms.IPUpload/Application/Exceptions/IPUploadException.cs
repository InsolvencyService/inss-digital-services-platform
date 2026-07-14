using System.Diagnostics.CodeAnalysis;

namespace Inss.GovUk.Forms.IPUpload.Application.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class IPUploadException : Exception
{
    internal IPUploadException(string message) : base(message)
    {
    }
}