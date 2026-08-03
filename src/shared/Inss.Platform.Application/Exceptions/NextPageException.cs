namespace Inss.Platform.Application.Exceptions;

public sealed class NextPageException : Exception
{
    public NextPageException(string message) : base(message)
    {
    }
}