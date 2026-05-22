namespace Inss.Auth.RpsProvider.Exceptions;

internal sealed class RpsProviderException : Exception
{
    internal RpsProviderException(string message) : base(message)
    {
    }
}