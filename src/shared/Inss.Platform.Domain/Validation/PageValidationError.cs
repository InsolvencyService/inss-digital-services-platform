namespace Inss.Platform.Domain.Validation;

public sealed class PageValidationError
{
    public required string[] Properties { get; init; }
    
    public required string Message { get; init; }
}