namespace Inss.Platform.Domain.Validation;

public sealed class Custom : ValidationBase
{
    public required Type ValidatorType { get; init; }
}