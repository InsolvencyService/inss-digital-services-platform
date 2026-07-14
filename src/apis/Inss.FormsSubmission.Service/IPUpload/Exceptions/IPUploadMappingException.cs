using System.Diagnostics.CodeAnalysis;

namespace Inss.FormsSubmission.Service.IPUpload.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class IPUploadMappingException : Exception
{
    internal IPUploadMappingException(string message) : base(message)
    {
    }
}