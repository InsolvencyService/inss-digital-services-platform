using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Application.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class FlowchartException : Exception
{
    internal FlowchartException(string message) : base(message)
    {
    }
}