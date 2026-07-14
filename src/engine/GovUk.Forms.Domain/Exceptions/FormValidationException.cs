using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class FormValidationException : Exception
{
    internal FormValidationException(List<string> messages) : base(string.Join(Environment.NewLine, messages))
    {
    }
}