namespace INSS.Platform.Portal.Domain.Exceptions;

public sealed class FormModelException : Exception
{
    public FormModelException(string message) : base(message)
    {
    }
}
